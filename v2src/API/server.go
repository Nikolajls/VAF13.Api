package api

import (
	"VAF13/api/handlers"
	"VAF13/api/middleware"
	"github.com/go-chi/chi/v5"
	"net/http"
)

func Serve() {
	r := chi.NewRouter()

	r.Use(middleware.APIKeyMiddleware)

	r.Route("/api/Member", func(r chi.Router) {
		r.Get("/Person", handlers.GetPerson)       // ?personId=2080&club=VAF
		r.Get("/Search", handlers.GetSearch)       //?name=Nikolaj
		r.Get("/SearchAll", handlers.GetSearchAll) //?name = Nikolaj
	})

	err := http.ListenAndServe(":8080", r)
	if err != nil {
		panic(err)
	}
}
