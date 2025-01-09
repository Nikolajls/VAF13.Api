package Klubadmin

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/stretchr/testify/assert"
	"go.uber.org/zap"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"net/url"
	"testing"
)

// https://hassansin.github.io/Unit-Testing-http-client-in-Go

// RoundTripFunc is defined as a type that is in the end a function
type RoundTripFunc func(req *http.Request) *http.Response

// RoundTripper interface implementation with the
func (f RoundTripFunc) RoundTrip(req *http.Request) (*http.Response, error) {
	return f(req), nil
}

// NewTestClient returns *http.Client with Transport replaced to the specificed roundtripper impl roundtripfunc
func NewTestClient(fn RoundTripFunc) *http.Client {
	return &http.Client{
		Transport: RoundTripFunc(fn),
	}
}

func Test_AuthenticationWorks_WhenReturning200AndLoginResultWithPHPSessid(t *testing.T) {
	//Arrange
	expectedPhpSessionId := "666"
	expectedUsername := "username"
	expectedPassword := "password"

	responseObj := &LoginResult{
		Type:   "systemlogin",
		Result: 2080,
	}
	jsonData, err := json.Marshal(responseObj)
	if err != nil {
		log.Fatalf("Error serializing to JSON: %v", err)
	}
	jsonString := string(jsonData)

	responseHeaders := make(map[string][]string)
	responseHeaders["Set-Cookie"] = []string{fmt.Sprintf("PHPSESSID=%v", expectedPhpSessionId)}
	httpResponse := &http.Response{
		StatusCode: 200,
		Body:       ioutil.NopCloser(bytes.NewBufferString(jsonString)),
		Header:     responseHeaders,
	}

	client := NewTestClient(func(req *http.Request) *http.Response {
		return httpResponse
	})

	// Act
	s := &AuthIntegration{
		username:         expectedUsername,
		password:         expectedPassword,
		storedPhpSession: &SafeString{},
		logger:           zap.NewNop(),
		authClient:       client,
	}
	var service AuthService = s
	resultingPhpSessionId, err := service.Authenticate()

	//Assert
	assert.NoError(t, err, "Authentication failed")
	assert.NotEmpty(t, resultingPhpSessionId, "Authentication failed")
}

func Test_Authentication_ValidateRequest(t *testing.T) {
	//Arrange
	expectedUsername := "username"
	expectedPassword := "password"

	expectedDataRequestValues := url.Values{}
	expectedDataRequestValues.Set("loginid", expectedUsername)
	expectedDataRequestValues.Set("password", expectedPassword)
	expectedDataRequestValues.Set("action", "systemlogin")

	var actualHttpRequest *http.Request
	httpResponse := &http.Response{
		StatusCode: 200,
		Body:       ioutil.NopCloser(bytes.NewBufferString("")),
		Header:     make(http.Header),
	}

	client := NewTestClient(func(req *http.Request) *http.Response {
		actualHttpRequest = req
		return httpResponse
	})

	s := &AuthIntegration{
		username:         expectedUsername,
		password:         expectedPassword,
		storedPhpSession: &SafeString{},
		logger:           zap.NewNop(),
		authClient:       client,
	}

	// Act
	var service AuthService = s
	_, err := service.Authenticate()

	//Assert
	assert.Equal(t, http.MethodPost, actualHttpRequest.Method)

	requestBodyBytes, err := io.ReadAll(actualHttpRequest.Body)
	assert.NoError(t, err)
	requestBodyString := string(requestBodyBytes)
	assert.NotEmpty(t, requestBodyString)
	requestUrlValues, err := url.ParseQuery(requestBodyString)
	assert.EqualValues(t, expectedDataRequestValues, requestUrlValues)
}

func Test_AuthenticationFails_WhenReturning200AndLoginResultButNoPhpSessid(t *testing.T) {
	//Arrange
	responseObj := &LoginResult{
		Type:   "systemlogin",
		Result: 2080,
	}
	jsonData, err := json.Marshal(responseObj)
	if err != nil {
		log.Fatalf("Error serializing to JSON: %v", err)
	}
	jsonString := string(jsonData)

	responseHeaders := make(map[string][]string)
	responseHeaders["Set-Cookie"] = []string{""}
	httpResponse := &http.Response{
		StatusCode: 200,
		Body:       ioutil.NopCloser(bytes.NewBufferString(jsonString)),
		Header:     responseHeaders,
	}

	client := NewTestClient(func(req *http.Request) *http.Response {
		return httpResponse
	})

	s := &AuthIntegration{
		username:         "test",
		password:         "test_password",
		storedPhpSession: &SafeString{},
		logger:           zap.NewNop(),
		authClient:       client,
	}

	//Act
	var service AuthService = s
	resultingPhpSessionId, err := service.Authenticate()

	//Assert
	assert.Error(t, err, "Authentication worked and should not due to missing phpsessionid")
	assert.Emptyf(t, resultingPhpSessionId, "Authentication worked and should not due to missing phpsessionid")
}

func Test_AuthenticationFails_WhenReturning200ButLoginResultMinusOne(t *testing.T) {
	//Arrange
	responseObj := &LoginResult{
		Type:   "systemlogin",
		Result: -1,
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

	client := NewTestClient(func(req *http.Request) *http.Response {
		return httpResponse
	})

	s := &AuthIntegration{
		username:         "test",
		password:         "test_password",
		storedPhpSession: &SafeString{},
		logger:           zap.NewNop(),
		authClient:       client,
	}

	//Act
	var service AuthService = s
	resultingPhpSessionId, err := service.Authenticate()

	//Assert
	assert.Error(t, err, "Authentication worked and should not due to backend returning minus one")
	assert.Emptyf(t, resultingPhpSessionId, "Authentication worked and should not  due to backend returning minus one")
}

func Test_AuthenticationFails_WhenReturning200ButFaultyLoginResult(t *testing.T) {
	//Arrange
	httpResponse := &http.Response{
		StatusCode: 200,
		Body:       ioutil.NopCloser(bytes.NewBufferString("invalidJson")),
		Header:     make(http.Header),
	}

	client := NewTestClient(func(req *http.Request) *http.Response {
		return httpResponse
	})

	s := &AuthIntegration{
		username:         "test",
		password:         "test_password",
		storedPhpSession: &SafeString{},
		logger:           zap.NewNop(),
		authClient:       client,
	}

	//Act
	var service AuthService = s
	resultingPhpSessionId, err := service.Authenticate()

	//Assert
	assert.Error(t, err, "Authentication worked and should not due to backend handling faulty response")
	assert.Emptyf(t, resultingPhpSessionId, "Authentication worked and should not  due to backend  handling faulty response")
}

func Test_AuthenticationFails_WhenReturningNon200(t *testing.T) {
	//Arrange
	httpResponse := &http.Response{
		StatusCode: 401,
		Body:       ioutil.NopCloser(bytes.NewBufferString("invalidJson")),
		Header:     make(http.Header),
	}

	client := NewTestClient(func(req *http.Request) *http.Response {
		return httpResponse
	})

	s := &AuthIntegration{
		username:         "test",
		password:         "test_password",
		storedPhpSession: &SafeString{},
		logger:           zap.NewNop(),
		authClient:       client,
	}

	//Act
	var service AuthService = s
	resultingPhpSessionId, err := service.Authenticate()

	// Assert
	assert.Error(t, err, "Authentication worked and should not due to non 200")
	assert.Emptyf(t, resultingPhpSessionId, "Authentication worked and should not 200")
}
