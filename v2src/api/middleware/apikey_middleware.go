package middleware

import (
	"VAF13/api/configs"
	"go.uber.org/zap"
	"net/http"
)

type APIKeyMiddleware struct {
	logger *zap.Logger
	apiKey string
}

func NewAPIKeyMiddleware(logger *zap.Logger, configuration configs.ConfigurationProvider) *APIKeyMiddleware {
	return &APIKeyMiddleware{
		logger: logger,
		apiKey: configuration.ApiKey(),
	}
}
func (s *APIKeyMiddleware) Add(next http.Handler) http.Handler {
	s.logger.Debug("Added APIKeyMiddleware")

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		correlationID, _ := ctx.Value("correlationID").(string)
		logger := s.logger.With(zap.String("correlation_id", correlationID))
		logger.Debug("Running APIKeyMiddleware")

		apiKeyFromHeader := r.Header.Get("X-API-Key")
		if apiKeyFromHeader == "" {
			apiKeyFromHeader = r.URL.Query().Get("api_key")
		}

		if apiKeyFromHeader == "" {
			logger.Warn("Api Key not found in header")
			http.Error(w, "Unauthorized: No API Key provided", http.StatusBadRequest)
			return
		}

		if s.apiKey == "" {
			logger.Warn("Api Key not configured")
			http.Error(w, "Unauthorized: Invalid API Key configuration", http.StatusInternalServerError)
			return
		}

		if apiKeyFromHeader != s.apiKey {
			logger.Warn("Api Key does not match API key configuration", zap.String("api_key", apiKeyFromHeader), zap.String("api_key_config", s.apiKey))
			http.Error(w, "Unauthorized: Invalid API Key", http.StatusUnauthorized)
			return
		}

		next.ServeHTTP(w, r)
	})
}
