package middleware

import (
	"VAF13/api/configs"
	"go.uber.org/zap"
	"net/http"
	"strings"
)

type CorsMiddleware struct {
	logger  *zap.Logger
	origins []string
}

func NewCorsMiddleware(logger *zap.Logger, configuration configs.ConfigurationProvider) *CorsMiddleware {
	return &CorsMiddleware{logger: logger, origins: configuration.CorsOrigins()}
}

func (s *CorsMiddleware) Add(next http.Handler) http.Handler {
	s.logger.Debug("Added CorsMiddleware")

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		correlationID, _ := ctx.Value("correlationID").(string)
		logger := s.logger.With(zap.String("correlation_id", correlationID))

		logger.Debug("Running CorsMiddleware")

		origins := strings.Join(s.origins, ",")
		logger.Debug("Adding cors headers", zap.String("origins", origins))

		w.Header().Add("Connection", "keep-alive")
		w.Header().Add("Access-Control-Allow-Origin", origins)
		w.Header().Add("Access-Control-Allow-Methods", "POST, OPTIONS, GET, DELETE, PUT")
		w.Header().Add("Access-Control-Allow-Headers", "*")
		w.Header().Add("Access-Control-Max-Age", "86400")
		if r.Method != "OPTIONS" {
			logger.Debug("Not options, continue")
			next.ServeHTTP(w, r)
		}
	})
}
