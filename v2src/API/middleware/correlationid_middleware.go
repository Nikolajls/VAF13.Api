package middleware

import (
	"context"
	"github.com/google/uuid"
	"go.uber.org/zap"
	"net/http"
)

const correlationIDKey = "correlationID"

type CorrelationIdMiddleware struct {
	logger *zap.Logger
}

func NewCorrelationIdMiddleware(logger *zap.Logger) *CorrelationIdMiddleware {
	return &CorrelationIdMiddleware{logger: logger}
}

// CorrelationIDMiddleware extracts the correlation ID from the request and injects it into the context.
func (s *CorrelationIdMiddleware) Add(next http.Handler) http.Handler {
	s.logger.Debug("Added CorrelationIdMiddleware")

	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		s.logger.Debug("Running CorrelationIdMiddleware")

		correlationID := r.Header.Get("Correlation-Id")

		logger := s.logger.With(zap.String("correlation_id", correlationID))

		if correlationID == "" {
			correlationID = uuid.NewString()
			logger = s.logger.With(zap.String("correlation_id", correlationID))
			logger.Info("No correlation id was found, it has been added")
		} else {
			logger.Info("Correlation id was found")
		}

		w.Header().Set("Correlation-Id", correlationID)
		ctx := context.WithValue(r.Context(), correlationIDKey, correlationID)
		next.ServeHTTP(w, r.WithContext(ctx))
	})
}
