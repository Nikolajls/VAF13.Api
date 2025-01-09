package Klubadmin

import (
	"bytes"
	"encoding/json"
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
	//MockConvertSearchResultToResponseResult *SearchResultResponse
}

func (service *MockMappingService) ConvertSearchResultToResponse(person *SearchResultPerson) (*SearchResultResponse, error) {
	return &SearchResultResponse{
		Name: person.Name,
	}, nil
}

func (service *MockMappingService) ConvertHtmlPersonToPerson(personId int, personHtmlDetails string) (*PersonResponse, error) {
	return nil, nil
}

func Test_Search(t *testing.T) {
	var authMockService AuthService = &MockAuthService{returnValue: "TEST"}
	var mappingMockService MappingService = &MockMappingService{}

	searchResultPerson := SearchResultPerson{
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
		Data:            []SearchResultPerson{searchResultPerson},
	}

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

	res, err := service.Search("Nikolaj")
	assert.NoError(t, err)
	assert.NotNil(t, res)
	assert.NotNil(t, actualRequest)
}
