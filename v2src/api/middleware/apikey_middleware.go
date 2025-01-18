package middleware

import (
	"go.uber.org/zap"
	"net/http"
	"os"
)

type APIKeyMiddleware struct {
	logger *zap.Logger
	apiKey string
}

func NewAPIKeyMiddleware(logger *zap.Logger) *APIKeyMiddleware {
	return &APIKeyMiddleware{
		logger: logger,
		apiKey: os.Getenv("ApiConfiguration__ApiKey"),
	}
}
func (s *APIKeyMiddleware) Add(next http.Handler) http.Handler {
	s.logger.Debug("Added APIKeyMiddleware")

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		s.logger.Debug("Running APIKeyMiddleware")

		apiKeyFromHeader := r.Header.Get("X-API-Key")
		if apiKeyFromHeader == "" {
			apiKeyFromHeader = r.URL.Query().Get("api_key")
		}

		if s.apiKey == "" {
			http.Error(w, "Unauthorized: Invalid API Key configuration", http.StatusInternalServerError)
			return
		}

		if apiKeyFromHeader != s.apiKey {
			http.Error(w, "Unauthorized: Invalid API Key", http.StatusUnauthorized)
			return
		}

		next.ServeHTTP(w, r)
	})
}
