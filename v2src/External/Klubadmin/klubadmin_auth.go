package Klubadmin

import (
	"VAF13/Helpers"
	"bytes"
	"fmt"
	"net/http"
	"net/url"
	"sync"
)

var authClient *http.Client = &http.Client{}
var storedPhpSession *SafeString = &SafeString{}
var username string = ""
var password string = ""

type SafeString struct {
	mu  sync.RWMutex
	val string
}

func (s *SafeString) Set(newValue string) {
	s.mu.Lock()
	defer s.mu.Unlock()
	s.val = newValue
}

func (s *SafeString) Get() string {
	s.mu.RLock() // Use RLock for readers
	defer s.mu.RUnlock()
	return s.val
}

func Authenticate() (string, error) {
	readSession := storedPhpSession.Get()
	if readSession != "" {
		return readSession, nil
	}

	headers := map[string]string{
		"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
		"User-Agent":   userAgent,
	}
	data := url.Values{}
	data.Set("loginid", username)
	data.Set("password", password)
	data.Set("action", "systemlogin")
	bodyT := bytes.NewBufferString(data.Encode())
	body := bodyT.Bytes()

	requestUrl := fmt.Sprintf("%s%s", baseUrl, "/")
	var resp, _, err = Helpers.MakeHttpRequest(authClient, "POST", requestUrl, headers, body)

	if err != nil {
		return "", fmt.Errorf("Error making request: %v", err)
	}

	//Verify statusCode
	if resp.StatusCode != 200 {
		return "", fmt.Errorf("Auth returned non successful statuscode")
	}

	//Verify signin worked
	loginResult, _, err := Helpers.ParseJSONResponse[LoginResult](resp)
	if err != nil {
		return "", fmt.Errorf("Error parsing response: %v", err)
	}

	if loginResult.Result <= 0 {
		return "", fmt.Errorf("Unable to signin")
	}

	//Extract PhpSessionId
	setCookieHeader := resp.Header.Get("Set-Cookie")
	cookieMap := Helpers.ConvertHttpHeaderValueToMap(setCookieHeader)
	if value, exists := cookieMap["PHPSESSID"]; exists {
		storedPhpSession.Set(value)
		return value, nil
	} else {
		return "", fmt.Errorf("PHPSESSID not found")
	}
}
