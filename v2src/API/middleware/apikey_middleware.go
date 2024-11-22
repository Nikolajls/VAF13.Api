package middleware

import (
	"net/http"
	"os"
)

var apiKey = os.Getenv("API_KEY")

func APIKeyMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {

		apiKeyFromHeader := r.Header.Get("X-API-Key")
		if apiKeyFromHeader == "" {
			apiKeyFromHeader = r.URL.Query().Get("api_key")
		}

		if apiKey == "" {
			http.Error(w, "Unauthorized: Invalid API Key configuration", http.StatusInternalServerError)
			return
		}

		if apiKeyFromHeader != apiKey {
			http.Error(w, "Unauthorized: Invalid API Key", http.StatusUnauthorized)
			return
		}

		next.ServeHTTP(w, r)
	})
}
