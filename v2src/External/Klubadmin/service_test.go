package Klubadmin

import (
	"bytes"
	"github.com/stretchr/testify/assert"
	"go.uber.org/zap"
	"io/ioutil"
	"net/http"
	"testing"
)

type MockAuthService struct {
	returnValue string
}

func (mockService *MockAuthService) Authenticate() (string, error) {
	return mockService.returnValue, nil
}

type MockMappingService struct{}

func (service *MockMappingService) ConvertSearchResultToResponse(person *SearchResultPerson) SearchResultResponse {
	return SearchResultResponse{}
}

func (service *MockMappingService) ConvertHtmlPersonToPerson(personId int, personHtmlDetails string) (*PersonResponse, error) {
	return nil, nil
}

func Test_Search(t *testing.T) {
	var authMockService AuthService = &MockAuthService{returnValue: "TEST"}
	var mappingMockService MappingService = &MockMappingService{}
	httpResponse := &http.Response{
		StatusCode: 200,
		Body:       ioutil.NopCloser(bytes.NewBufferString("JSON")),
		Header:     make(http.Header),
	}

	var actualRequest *http.Request
	service := NewDefaultKlubAdminService(zap.NewNop(), authMockService, mappingMockService, RoundTripFunc(func(req *http.Request) *http.Response {
		actualRequest = req
		return httpResponse
	}))

	res, err := service.Search("AA")
	assert.Error(t, err)
	assert.NotNil(t, res)
	assert.NotNil(t, actualRequest)
}
