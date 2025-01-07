package Klubadmin

import (
	"VAF13/Helpers"
	"bytes"
	"fmt"
	"go.uber.org/zap"
	"net/http"
	"net/url"
	"os"
	"sync"
	"time"
)

type Klubadmin_integration_auth struct {
	logger *zap.Logger
}

func NewKlubadmin_integration_auth(logger *zap.Logger) *Klubadmin_integration_auth {
	return &Klubadmin_integration_auth{logger: logger}
}

var authClient *http.Client = &http.Client{}
var storedPhpSession *SafeString = &SafeString{}
var username string = os.Getenv("DfuConfiguration_Username")
var password string = os.Getenv("DfuConfiguration_Password")

type SafeString struct {
	mu         sync.RWMutex
	val        string
	cachedTime time.Time
}

func (s *SafeString) Set(newValue string, cachedTime time.Time) {
	s.mu.Lock()
	defer s.mu.Unlock()
	s.val = newValue
	s.cachedTime = cachedTime
}

func (s *SafeString) Get() (string, time.Time) {
	s.mu.RLock() // Use RLock for readers
	defer s.mu.RUnlock()
	return s.val, s.cachedTime
}

func (s *Klubadmin_integration_auth) Authenticate() (string, error) {
	readSession, cachedTime := storedPhpSession.Get()
	if readSession != "" {
		cacheDiffMinutes := time.Now().Sub(cachedTime).Minutes()
		if cacheDiffMinutes < 10 {
			return readSession, nil
		} else {
			s.logger.Info("Refreshing cached authentication key")
		}
	} else {
		s.logger.Info("No cached authentication yet")
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
		s.logger.Error("Authentication Failed", zap.Int("StatusCode", resp.StatusCode), zap.String("url", requestUrl))
		return "", fmt.Errorf("Auth returned non successful statuscode")
	}

	//Verify signin worked
	loginResult, _, err := Helpers.ParseJSONResponse[LoginResult](resp)
	if err != nil {
		s.logger.Error("Error parsing json response", zap.String("error", err.Error()))
		return "", fmt.Errorf("Error parsing response: %v", err)
	}

	if loginResult.Result <= 0 {
		return "", fmt.Errorf("Unable to signin")
	}
	s.logger.Info("Successfully logged in")
	//Extract PhpSessionId
	setCookieHeader := resp.Header.Get("Set-Cookie")
	cookieMap := Helpers.ConvertHttpHeaderValueToMap(setCookieHeader)
	if value, exists := cookieMap["PHPSESSID"]; exists {
		s.logger.Info("Setting cached authentication")
		storedPhpSession.Set(value, time.Now())
		return value, nil
	} else {
		s.logger.Error("Authentication succeeded but no PHP session id")
		return "", fmt.Errorf("PHPSESSID not found")
	}
}
