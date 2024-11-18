package Helpers

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"strings"
)

func MakeHttpRequest(client *http.Client, method string, url string, headers map[string]string, data []byte) (*http.Response, *http.Request, error) {
	req, err := http.NewRequest(method, url, bytes.NewBuffer(data))
	if err != nil {
		return nil, nil, err
	}

	for key, value := range headers {
		req.Header.Set(key, value)
	}

	res, err := client.Do(req)

	if err != nil {
		return nil, nil, err
	}

	return res, req, nil
}

func ConvertHttpHeaderValueToMap(headerValue string) map[string]string {
	parts := strings.Split(headerValue, ";")

	headerMap := make(map[string]string) // Create a map to store key-value pairs

	for _, part := range parts { // Process each part
		part = strings.TrimSpace(part) // Trim whitespace from the part

		if keyValue := strings.SplitN(part, "=", 2); len(keyValue) == 2 { // Split on `=` to get key and value
			// Add the key-value pair to the map
			headerMap[keyValue[0]] = keyValue[1]
		} else {
			// If no `=`, it's a standalone key (e.g., HttpOnly)
			headerMap[part] = ""
		}
	}
	return headerMap
}

func ReadHttpResponseAsString(resp *http.Response) (string, error) {
	defer resp.Body.Close()

	bodyBytes, err := io.ReadAll(resp.Body)
	if err != nil {
		fmt.Println("Error reading body:", err)
		return "", fmt.Errorf("Error reading body: %v", err)
	}

	// Convert the body to a string
	bodyString := string(bodyBytes)
	return bodyString, nil
}

func ParseJSONResponse[T any](resp *http.Response) (*T, string, error) {
	// Ensure the response body is closed after reading
	defer resp.Body.Close()

	// Read the body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, "", fmt.Errorf("failed to read response body: %w", err)
	}
	bodyString := string(body)

	// Parse the JSON into the generic struct
	var result T
	err = json.Unmarshal(body, &result)
	if err != nil {
		return nil, "", fmt.Errorf("failed to unmarshal JSON: %w", err)
	}

	return &result, bodyString, nil
}
