package Klubadmin

import (
	"VAF13/Helpers"
	"bytes"
	"fmt"
	"go.uber.org/zap"
	"net/http"
	"net/url"
	"strconv"
	"strings"
	"sync"
	"time"
)

type Service interface {
	Search(name string) ([]SearchResultResponse, error)
	SearchAll(name string) ([]PersonResponse, error)
	GetPerson(personId int) (*PersonResponse, error)
}

type Integration struct {
	logger         *zap.Logger
	KlubAdminAuth  AuthService
	MappingService MappingService
	baseUrl        string
	userAgent      string
	client         *http.Client
}

func NewDefaultKlubAdminService(logger *zap.Logger, klubAdminAuthService AuthService, mappingService MappingService, finalRoundTripper http.RoundTripper) Service {
	if finalRoundTripper == nil {
		finalRoundTripper = http.DefaultTransport
	}

	client := &http.Client{
		Transport: &AuthRoundTripper{
			AuthService: klubAdminAuthService,
			Transport:   finalRoundTripper,
		},
	}

	service := Integration{
		logger:         logger,
		MappingService: mappingService,
		KlubAdminAuth:  klubAdminAuthService,
		client:         client,
		baseUrl:        "https://klubadmin.dfu.dk/klubadmin/pages",
		userAgent:      "VAFAPI/2.0",
	}

	return &service
}

type AuthRoundTripper struct {
	Transport   http.RoundTripper
	AuthService AuthService
}

func (a *AuthRoundTripper) RoundTrip(req *http.Request) (*http.Response, error) {
	clonedReq := req.Clone(req.Context())

	phpSessionId, err := a.AuthService.Authenticate()
	if err != nil {
		return nil, err
	}
	// Add the Authorization header to the cloned request
	cookie := fmt.Sprintf("PHPSESSID=%s", phpSessionId)
	clonedReq.Header.Add("Cookie", cookie)

	// Forward the cloned request to the underlying transport
	return a.Transport.RoundTrip(clonedReq)
}

func (service *Integration) SearchAll(name string) ([]PersonResponse, error) {
	var resultSlice []PersonResponse

	persons, err := service.Search(name)
	if err != nil {
		return nil, err
	}
	personCount := len(persons)
	var wg sync.WaitGroup
	results := make(chan PersonResponse, personCount)
	errors := make(chan error, personCount)
	semaphore := make(chan struct{}, 10)

	start := time.Now()
	for _, person := range persons {
		wg.Add(1)
		go fetchDetail(service, person, &wg, semaphore, results, errors)
	}

	wg.Wait() // Wait for all goroutines to complete
	close(results)
	close(errors)

	for result := range results { // Process results
		resultSlice = append(resultSlice, result)
	}

	for err := range errors { // Process errors
		service.logger.Error("SearchAll Failed to do search for specific person", zap.Error(err))
		fmt.Println("Errors for SearchAll", err)
	}

	service.logger.Info(fmt.Sprintf("Total time taken:%v", time.Since(start).Seconds()))
	return resultSlice, nil
}

func fetchDetail(service *Integration, person SearchResultResponse, wg *sync.WaitGroup, semaphore chan struct{}, results chan<- PersonResponse, errors chan<- error) {
	defer wg.Done()

	// Acquire a semaphore slot
	semaphore <- struct{}{}
	defer func() { <-semaphore }() // Release the semaphore slot

	personResult, err := service.GetPerson(person.Id)
	if err != nil {
		errors <- err
		return
	}

	// Send the result to the results channel
	results <- *personResult
}

func (service *Integration) Search(name string) ([]SearchResultResponse, error) {
	now := time.Now()
	epochSeconds := now.Unix()
	nameEscaped := url.QueryEscape(name)

	var queryUrl = "/p_members/server_processing.php?draw=12&start=0&length=-1&search%5Bvalue%5D={nameEncoded}&search%5Bregex%5D=false&_t={secondsSinceEpoch}"
	queryUrl = strings.Replace(queryUrl, "{nameEncoded}", nameEscaped, -1)
	queryUrl = strings.Replace(queryUrl, "{secondsSinceEpoch}", strconv.FormatInt(epochSeconds, 10), -1)

	requestUrl := fmt.Sprintf("%s%s", service.baseUrl, queryUrl)
	headers := map[string]string{
		"User-Agent": service.userAgent,
	}

	timeNow := time.Now()
	var resp, _, err = Helpers.MakeHttpRequest(service.client, "GET", requestUrl, headers, nil)

	if err != nil {
		return make([]SearchResultResponse, 0), fmt.Errorf("error making SearchPerson request: %v", err)
	}

	if resp.StatusCode != 200 {
		return make([]SearchResultResponse, 0), fmt.Errorf("search for person returned non successful statuscode")
	}

	searchResponse, _, err := Helpers.ParseJSONResponse[SearchResult](resp)
	if err != nil {
		return make([]SearchResultResponse, 0), fmt.Errorf("Error parsing response to json for search %v", err)
	}

	searchPersonCount := len(searchResponse.Data)
	service.logger.Info(fmt.Sprintf("Search time for %v took %v seconds and gave %v persons", name, time.Since(timeNow).Seconds(), searchPersonCount))
	personsResponse := make([]SearchResultResponse, searchPersonCount)
	for index := range searchResponse.Data {
		ptr := &searchResponse.Data[index]
		ptr.CleanupResult()

		if mappedPerson, mappedError := service.MappingService.ConvertSearchResultToResponse(ptr); mappedError == nil && mappedPerson != nil {
			personsResponse[index] = *mappedPerson
		}
	}

	return personsResponse, nil
}

func (service *Integration) GetPerson(personId int) (*PersonResponse, error) {
	headers := map[string]string{
		"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
		"User-Agent":   service.userAgent,
	}

	data := url.Values{}
	personIdString := strconv.Itoa(personId)
	data.Set("personId", personIdString)
	data.Set("group", "personInfo")
	bodyT := bytes.NewBufferString(data.Encode())
	body := bodyT.Bytes()

	requestUrl := fmt.Sprintf("%s%s", service.baseUrl, "/ajax.php")
	var resp, _, err = Helpers.MakeHttpRequest(service.client, "POST", requestUrl, headers, body)

	if err != nil {
		return nil, fmt.Errorf("error making GetPerson request for personId: %v", personId)
	}

	//Verify statusCode
	if resp.StatusCode != 200 {
		return nil, fmt.Errorf("getPerson returned non successful statuscode: %v for personId:%v", resp.StatusCode, personId)
	}

	personHtmlDetails, err := Helpers.ReadHttpResponseAsString(resp)
	if err != nil {
		return nil, err
	}
	resultingPerson, err := service.MappingService.ConvertHtmlPersonToPerson(personId, personHtmlDetails)
	if err != nil {
		return nil, err
	}

	resultingPerson.CleanupResult()
	return resultingPerson, nil
}
