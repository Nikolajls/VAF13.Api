package middleware

import (
	"go.uber.org/zap"
	"net/http"
	"time"
)

type RequestLoggingMiddleware struct {
	logger *zap.Logger
}

func NewRequestLoggingMiddleware(logger *zap.Logger) *RequestLoggingMiddleware {
	return &RequestLoggingMiddleware{logger: logger}
}

func (s *RequestLoggingMiddleware) Add(next http.Handler) http.Handler {
	s.logger.Debug("Added RequestLoggingMiddleware")
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		s.logger.Debug("Running RequestLoggingMiddleware")

		start := time.Now()

		ctx := r.Context()
		correlationID, _ := ctx.Value("correlationID").(string)
		logger := s.logger.With(
			zap.String("correlation_id", correlationID),
			zap.String("method", r.Method),
			zap.String("url", r.URL.String()),
			zap.String("remote_addr", r.RemoteAddr))

		logger.Info("handling request")

		next.ServeHTTP(w, r)

		logger.Info("handled request", zap.Duration("duration", time.Since(start)))
	})
}
