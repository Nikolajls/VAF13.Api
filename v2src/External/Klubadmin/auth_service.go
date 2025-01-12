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

type AuthService interface {
	Authenticate() (string, error)
}

type AuthIntegration struct {
	logger           *zap.Logger
	authClient       *http.Client
	storedPhpSession *SafeString
	username         string
	password         string
	baseUrl          string
	userAgent        string
}

func NewAuthService(logger *zap.Logger, httpClientForAuth *http.Client) AuthService {
	if httpClientForAuth == nil {
		httpClientForAuth = &http.Client{}
	}

	service := AuthIntegration{
		username:         os.Getenv("DfuConfiguration_Username"),
		password:         os.Getenv("DfuConfiguration_Password"),
		storedPhpSession: &SafeString{},
		logger:           logger,
		authClient:       httpClientForAuth,
		baseUrl:          "https://klubadmin.dfu.dk/klubadmin/pages",
		userAgent:        "VAFAPI/2.0",
	}
	a := &service

	return a
}

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

func (s *AuthIntegration) Authenticate() (string, error) {
	readSession, cachedTime := s.storedPhpSession.Get()
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
		"User-Agent":   s.userAgent,
	}
	data := url.Values{}
	data.Set("loginid", s.username)
	data.Set("password", s.password)
	data.Set("action", "systemlogin")
	bodyT := bytes.NewBufferString(data.Encode())
	body := bodyT.Bytes()

	requestUrl := fmt.Sprintf("%s%s", s.baseUrl, "/")
	var resp, _, err = Helpers.MakeHttpRequest(s.authClient, "POST", requestUrl, headers, body)

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
		s.storedPhpSession.Set(value, time.Now())
		return value, nil
	} else {
		s.logger.Error("Authentication succeeded but no PHP session id")
		return "", fmt.Errorf("PHPSESSID not found")
	}
}
