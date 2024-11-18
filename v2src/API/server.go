package api

import (
	"VAF13/api/handlers"
	"github.com/go-chi/chi/v5"
	"net/http"
)

func Serve() {
	r := chi.NewRouter()

	r.Route("/", func(r chi.Router) {
		r.Get("/", handlers.Index)
	})

	err := http.ListenAndServe(":8080", r)
	if err != nil {
		panic(err)
	}
}
