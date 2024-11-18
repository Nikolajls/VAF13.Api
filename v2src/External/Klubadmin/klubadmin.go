package Klubadmin

import (
	"VAF13/Helpers"
	"bytes"
	"fmt"
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

func SearchAll(name string) ([]PersonResult, error) {
	var resultSlice []PersonResult

	persons, err := Search(name)
	if err != nil {
		return nil, err
	}

	personCount := len(persons)
	var wg sync.WaitGroup
	results := make(chan PersonResult, personCount)
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

func fetchDetail(person SearchResultPerson, wg *sync.WaitGroup, semaphore chan struct{}, results chan<- PersonResult, errors chan<- error) {
	defer wg.Done()

	// Acquire a semaphore slot
	semaphore <- struct{}{}
	defer func() { <-semaphore }() // Release the semaphore slot

	personResult, err := GetPerson(person)
	if err != nil {
		errors <- err
		return
	}

	// Send the result to the results channel
	results <- *personResult
}

func Search(name string) ([]SearchResultPerson, error) {
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
		return make([]SearchResultPerson, 0), fmt.Errorf("Error making SearchPerson request: %v", err)
	}

	if resp.StatusCode != 200 {
		return make([]SearchResultPerson, 0), fmt.Errorf("Search for person returned non successful statuscode")
	}

	searchResponse, _, _ := Helpers.ParseJSONResponse[SearchResult](resp)
	for index := range searchResponse.Data {
		ptr := &searchResponse.Data[index]
		ptr.CleanupResult()
	}

	return searchResponse.Data, nil
}

func GetPerson(person SearchResultPerson) (*PersonResult, error) {

	headers := map[string]string{
		"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
		"User-Agent":   userAgent,
	}

	data := url.Values{}
	personIdString := strconv.Itoa(person.Id)
	data.Set("personId", personIdString)
	data.Set("group", "personInfo")
	bodyT := bytes.NewBufferString(data.Encode())
	body := bodyT.Bytes()

	requestUrl := fmt.Sprintf("%s%s", baseUrl, "/ajax.php")
	var resp, _, err = Helpers.MakeHttpRequest(client, "POST", requestUrl, headers, body)

	if err != nil {
		return nil, fmt.Errorf("Error making GetPerson request: %v for name:%v  personId:%v", err, person.Name, person.Id)
	}

	//Verify statusCode
	if resp.StatusCode != 200 {
		return nil, fmt.Errorf("GetPerson returned non successful statuscode: %v for namme:%v  personId:%v", resp.StatusCode, person.Name, personIdString)
	}

	personHtmlDetails, err := Helpers.ReadHttpResponseAsString(resp)

	resultingPerson := convertHtmlPersonToPerson(person, personHtmlDetails)
	resultingPerson.CleanupResult()
	return resultingPerson, nil
}

func convertHtmlPersonToPerson(person SearchResultPerson, personHtmlDetails string) *PersonResult {
	doc, err := html.Parse(strings.NewReader(personHtmlDetails))
	if err != nil {
		fmt.Errorf("Unable to parse html to html node")
	}

	var fullName string = Helpers.GetElementAttributeValue(doc, "person_firstname", "value")
	nameSplit := strings.Split(fullName, " ")

	var result PersonResult = PersonResult{
		Id:              strconv.Itoa(person.Id),
		FirstName:       nameSplit[0],
		LastName:        strings.Join(nameSplit[1:], " "),
		Address:         Helpers.GetElementAttributeValue(doc, "person_address", "value"),
		Zip:             Helpers.GetElementAttributeValue(doc, "person_zip", "value"),
		Club:            person.Club,
		City:            Helpers.GetElementAttributeValue(doc, "person_city", "value"),
		Country:         Helpers.GetElementAttributeValue(doc, "person_country", "data-orgvalue"),
		Mail:            Helpers.GetElementAttributeValue(doc, "person_mail", "value"),
		Phone:           Helpers.GetElementAttributeValue(doc, "person_cellular", "value"),
		Birthday:        Helpers.GetElementAttributeValue(doc, "person_birthdayDate_inverted", "value"),
		ContactName:     Helpers.GetElementAttributeValue(doc, "relative_firstname", "value"),
		ContactPhone:    Helpers.GetElementAttributeValue(doc, "relative_cellular", "value"),
		ContactRelation: Helpers.GetElementAttributeValue(doc, "person_relativerelation", "value"),
		Gender:          Helpers.GetElementAttributeValue(doc, "gender", "data-orgvalue"),
		Certificate:     Helpers.GetElementAttributeValue(doc, "certificateNr_3", "data-orgvalue"),
	}
	return &result
}
