package middleware

import (
	"go.uber.org/zap"
	"net/http"
	"os"
)

var apiKey = os.Getenv("API_KEY")

type APIKeyMiddleware struct {
	logger *zap.Logger
}

func NewAPIKeyMiddleware(logger *zap.Logger) *APIKeyMiddleware {
	return &APIKeyMiddleware{logger: logger}
}
func (s *APIKeyMiddleware) Add(next http.Handler) http.Handler {
	s.logger.Debug("Added APIKeyMiddleware")

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		s.logger.Debug("Running APIKeyMiddleware")

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
