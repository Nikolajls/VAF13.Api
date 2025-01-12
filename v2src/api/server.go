package api

import (
	"VAF13/External/Klubadmin"
	"VAF13/api/handlers"
	"VAF13/api/middleware"
	"github.com/go-chi/chi/v5"
	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
	"net/http"
)

func Serve() {
	r := chi.NewRouter()

	var err error
	encoderConfig := zapcore.EncoderConfig{
		TimeKey:        "timestamp",
		LevelKey:       "level",
		NameKey:        "logger",
		CallerKey:      "caller",
		MessageKey:     "msg",
		StacktraceKey:  "stacktrace",
		LineEnding:     zapcore.DefaultLineEnding,
		EncodeLevel:    zapcore.LowercaseLevelEncoder, // lowercase level names
		EncodeTime:     zapcore.ISO8601TimeEncoder,    // ISO8601 UTC timestamp format
		EncodeDuration: zapcore.StringDurationEncoder,
		EncodeCaller:   zapcore.ShortCallerEncoder,
	}

	// Create a custom logger configuration
	config := zap.Config{
		Level:            zap.NewAtomicLevelAt(zap.DebugLevel),
		Development:      false,
		Encoding:         "json",
		EncoderConfig:    encoderConfig,
		OutputPaths:      []string{"stdout"},
		ErrorOutputPaths: []string{"stderr"},
	}

	// Initialize the logger with the custom configuration
	logger, err := config.Build()
	if err != nil {
		panic(err)
	}
	defer logger.Sync() // flushes buffer, if any

	r.Use(middleware.NewCorrelationIdMiddleware(logger).Add)
	r.Use(middleware.NewRequestLoggingMiddleware(logger).Add)
	r.Use(middleware.NewAPIKeyMiddleware(logger).Add)

	KlubAdminAuthService := Klubadmin.NewAuthService(logger, &http.Client{})
	KlubAdminMappingService := Klubadmin.NewDefaultMappingService()
	KlubAdminService := Klubadmin.NewDefaultKlubAdminService(logger, KlubAdminAuthService, KlubAdminMappingService, http.DefaultTransport)

	memberHandlers := handlers.NewMemberHandlers(logger, &KlubAdminService)
	r.Route("/api/Member", func(r chi.Router) {
		r.Get("/Person", memberHandlers.GetPerson)       // ?personId=2080
		r.Get("/Search", memberHandlers.GetSearch)       //?name=Nikolaj
		r.Get("/SearchAll", memberHandlers.GetSearchAll) //?name = Nikolaj
	})

	err = http.ListenAndServe(":8080", r)
	if err != nil {
		panic(err)
	}
}
