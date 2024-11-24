package Klubadmin

import (
	"VAF13/Helpers"
	"bytes"
	"fmt"
	"github.com/antchfx/htmlquery"
	"golang.org/x/net/html"
	"net/http"
	"net/url"
	"strconv"
	"strings"
	"sync"
	"time"
)

var baseUrl string = "https://klubadmin.dfu.dk/klubadmin/pages"
var userAgent string = "VAFAPI/2.0"

type AuthMiddleware struct {
	Transport http.RoundTripper
}

var client *http.Client = &http.Client{
	Transport: &AuthMiddleware{
		Transport: http.DefaultTransport, // Use default transport as the base
	},
}

// RoundTrip safely clones the request and adds the Authorization header.
func (a *AuthMiddleware) RoundTrip(req *http.Request) (*http.Response, error) {
	// Clone the request to avoid modifying the original
	clonedReq := req.Clone(req.Context())

	phpSessionId, err := Authenticate()
	if err != nil {
		return nil, err
	}
	// Add the Authorization header to the cloned request
	cookie := fmt.Sprintf("PHPSESSID=%s", phpSessionId)
	clonedReq.Header.Add("Cookie", cookie)

	// Forward the cloned request to the underlying transport
	return a.Transport.RoundTrip(clonedReq)
}

func SearchAll(name string) ([]PersonResponse, error) {
	var resultSlice []PersonResponse

	persons, err := Search(name)
	if err != nil {
		return nil, err
	}

	personCount := len(persons)
	var wg sync.WaitGroup
	results := make(chan PersonResponse, personCount)
	errors := make(chan error, personCount)
	semaphore := make(chan struct{}, 10)

	for _, person := range persons {
		wg.Add(1)
		go fetchDetail(person, &wg, semaphore, results, errors)
	}

	wg.Wait() // Wait for all goroutines to complete
	close(results)
	close(errors)

	for result := range results { // Process results
		resultSlice = append(resultSlice, result)
	}

	for err := range errors { // Process errors
		fmt.Println("Errors for SearchAll", err)
	}

	return resultSlice, nil
}

func fetchDetail(person SearchResultResponse, wg *sync.WaitGroup, semaphore chan struct{}, results chan<- PersonResponse, errors chan<- error) {
	defer wg.Done()

	// Acquire a semaphore slot
	semaphore <- struct{}{}
	defer func() { <-semaphore }() // Release the semaphore slot

	personResult, err := GetPerson(person.Id)
	if err != nil {
		errors <- err
		return
	}

	// Send the result to the results channel
	results <- *personResult
}

func Search(name string) ([]SearchResultResponse, error) {
	now := time.Now()
	epochSeconds := now.Unix()
	nameEscaped := url.QueryEscape(name)

	var queryUrl = "/p_members/server_processing.php?draw=12&start=0&length=-1&search%5Bvalue%5D={nameEncoded}&search%5Bregex%5D=false&_t={secondsSinceEpoch}"
	queryUrl = strings.Replace(queryUrl, "{nameEncoded}", nameEscaped, -1)
	queryUrl = strings.Replace(queryUrl, "{secondsSinceEpoch}", strconv.FormatInt(epochSeconds, 10), -1)

	requestUrl := fmt.Sprintf("%s%s", baseUrl, queryUrl)
	headers := map[string]string{
		"User-Agent": userAgent,
	}

	var resp, _, err = Helpers.MakeHttpRequest(client, "GET", requestUrl, headers, nil)

	if err != nil {
		return make([]SearchResultResponse, 0), fmt.Errorf("error making SearchPerson request: %v", err)
	}

	if resp.StatusCode != 200 {
		return make([]SearchResultResponse, 0), fmt.Errorf("search for person returned non successful statuscode")
	}

	searchResponse, _, _ := Helpers.ParseJSONResponse[SearchResult](resp)
	personsResponse := make([]SearchResultResponse, len(searchResponse.Data))
	for index := range searchResponse.Data {
		ptr := &searchResponse.Data[index]
		ptr.CleanupResult()
		personsResponse[index] = SearchResultResponse{
			Name:        ptr.Name,
			Club:        ptr.Club,
			DateAdded:   ptr.DateAdded,
			DateRemoved: ptr.DateRemoved,
			Address:     ptr.Address,
			DFUNo:       ptr.DFUNo,
			Birthday:    ptr.Birthday,
			Phone:       ptr.Phone,
			Email:       ptr.Email,
			Type:        ptr.Type,
			Certificate: ptr.Certificate,
			Id:          ptr.Id,
		}

	}

	return personsResponse, nil
}

func GetPerson(personId int) (*PersonResponse, error) {
	headers := map[string]string{
		"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
		"User-Agent":   userAgent,
	}

	data := url.Values{}
	personIdString := strconv.Itoa(personId)
	data.Set("personId", personIdString)
	data.Set("group", "personInfo")
	bodyT := bytes.NewBufferString(data.Encode())
	body := bodyT.Bytes()

	requestUrl := fmt.Sprintf("%s%s", baseUrl, "/ajax.php")
	var resp, _, err = Helpers.MakeHttpRequest(client, "POST", requestUrl, headers, body)

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
	resultingPerson, err := convertHtmlPersonToPerson(personId, personHtmlDetails)
	if err != nil {
		return nil, err
	}

	resultingPerson.CleanupResult()
	return resultingPerson, nil
}

func convertHtmlPersonToPerson(personId int, personHtmlDetails string) (*PersonResponse, error) {
	doc, err := html.Parse(strings.NewReader(personHtmlDetails))
	if err != nil {
		return nil, fmt.Errorf("unable to parse html to html node")
	}

	var fullName string = Helpers.GetElementAttributeValue(doc, "person_firstname", "value")
	nameSplit := strings.Split(fullName, " ")

	//Certificate
	certificateValue := Helpers.GetElementAttributeValue(doc, "certificateNr_3", "data-orgvalue")
	certificate := 0
	if c, err := strconv.Atoi(certificateValue); err == nil {
		certificate = c
	}

	// Club
	clubElement := Helpers.GetElementById(doc, "currentMembershipsTable")
	clubNodenode := htmlquery.FindOne(clubElement, "//tbody/tr[1]/td[1]")
	clubText := htmlquery.InnerText(clubNodenode)

	var result PersonResponse = PersonResponse{
		Id:              personId,
		FirstName:       nameSplit[0],
		LastName:        strings.Join(nameSplit[1:], " "),
		Address:         Helpers.GetElementAttributeValue(doc, "person_address", "value"),
		Zip:             Helpers.GetElementAttributeValue(doc, "person_zip", "value"),
		Club:            clubText,
		City:            Helpers.GetElementAttributeValue(doc, "person_city", "value"),
		Country:         Helpers.GetElementAttributeValue(doc, "person_country", "data-orgvalue"),
		Email:           Helpers.GetElementAttributeValue(doc, "person_mail", "value"),
		Phone:           Helpers.GetElementAttributeValue(doc, "person_cellular", "value"),
		Birthday:        Helpers.GetElementAttributeValue(doc, "person_birthdayDate_inverted", "value"),
		ContactName:     Helpers.GetElementAttributeValue(doc, "relative_firstname", "value"),
		ContactPhone:    Helpers.GetElementAttributeValue(doc, "relative_cellular", "value"),
		ContactRelation: Helpers.GetElementAttributeValue(doc, "person_relativerelation", "value"),
		Gender:          Helpers.GetElementAttributeValue(doc, "gender", "data-orgvalue"),
		Certificate:     certificate,
	}
	return &result, nil
}
