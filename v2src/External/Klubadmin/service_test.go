package Klubadmin

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/stretchr/testify/assert"
	"go.uber.org/zap"
	"io/ioutil"
	"log"
	"net/http"
	"testing"
)

type MockAuthService struct {
	returnValue string
}

func (mockService *MockAuthService) Authenticate() (string, error) {
	return mockService.returnValue, nil
}

type MockMappingService struct {
	MockConvertSearchResultToResponseArgument *SearchResultPerson
	MockConvertSearchResultToResponseResult   *SearchResultResponse

	MockConvertHtmlPersonToPersonArgumentPersonId int
	MockConvertHtmlPersonToPersonArgumentHtml     string
	MockConvertHtmlPersonToPersonResult           *PersonResponse
}

func (service *MockMappingService) ConvertSearchResultToResponse(person *SearchResultPerson) (*SearchResultResponse, error) {
	service.MockConvertSearchResultToResponseArgument = person
	if person == nil {
		return nil, fmt.Errorf("person cannot be null")
	}
	return service.MockConvertSearchResultToResponseResult, nil
}

func (service *MockMappingService) ConvertHtmlPersonToPerson(personId int, personHtmlDetails string) (*PersonResponse, error) {
	service.MockConvertHtmlPersonToPersonArgumentPersonId = personId
	service.MockConvertHtmlPersonToPersonArgumentHtml = personHtmlDetails

	return service.MockConvertHtmlPersonToPersonResult, nil
}

func Test_Search(t *testing.T) {
	var authMockService AuthService = &MockAuthService{returnValue: "TEST"}

	searchResultPerson := &SearchResultPerson{
		DTRowClass:  "",
		Name:        "Nikolaj",
		Club:        "",
		DateAdded:   "",
		DateRemoved: "",
		Address:     "",
		DFUNo:       0,
		Birthday:    "",
		Phone:       "",
		Email:       "",
		Type:        "",
		Certificate: 0,
		Id:          0,
	}

	responseObj := &SearchResult{
		Draw:            0,
		RecordsTotal:    1,
		RecordsFiltered: 1,
		Data:            []SearchResultPerson{*searchResultPerson},
	}

	expectedPersonResult := &SearchResultResponse{
		Name: searchResultPerson.Name,
	}

	mappingMock := &MockMappingService{
		MockConvertSearchResultToResponseResult: expectedPersonResult,
	}
	var mappingMockService MappingService = mappingMock

	jsonData, err := json.Marshal(responseObj)
	if err != nil {
		log.Fatalf("Error serializing to JSON: %v", err)
	}
	jsonString := string(jsonData)

	httpResponse := &http.Response{
		StatusCode: 200,
		Body:       ioutil.NopCloser(bytes.NewBufferString(jsonString)),
		Header:     make(http.Header),
	}

	var actualRequest *http.Request
	service := NewDefaultKlubAdminService(zap.NewNop(), authMockService, mappingMockService, RoundTripFunc(func(req *http.Request) *http.Response {
		actualRequest = req
		return httpResponse
	}))

	//Act
	res, err := service.Search("Nikolaj")

	//Assert
	assert.NoError(t, err)
	assert.NotNil(t, res)
	assert.NotEmpty(t, res)

	assert.EqualValues(t, []SearchResultResponse{*expectedPersonResult}, res, "The resulting person search did not match the expected")
	assert.EqualValues(t, searchResultPerson, mappingMock.MockConvertSearchResultToResponseArgument)

	//Verify request
	assert.NotNil(t, actualRequest)
	//Verify auth
	//Verify request
}
