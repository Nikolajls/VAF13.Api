package handlers

import (
	"VAF13/External/Klubadmin"
	"encoding/json"
	"fmt"
	"github.com/go-chi/render"
	"go.uber.org/zap"
	"net/http"
	"strconv"
)

type MemberHandlers struct {
	logger    *zap.Logger
	klubadmin *Klubadmin.Klubadmin_integration
}

func NewMemberHandlers(logger *zap.Logger, integration *Klubadmin.Klubadmin_integration) *MemberHandlers {
	return &MemberHandlers{
		logger:    logger,
		klubadmin: integration,
	}
}

func (s *MemberHandlers) GetPerson(w http.ResponseWriter, request *http.Request) {
	ctx := request.Context()
	correlationID, _ := ctx.Value("correlationID").(string)
	logger := s.logger.With(zap.String("correlation_id", correlationID))

	personIdString := request.URL.Query().Get("personId")
	if personIdString == "" {
		http.Error(w, "Missing query parameter 'personId'", http.StatusBadRequest)
		return
	}

	personId, err := strconv.Atoi(personIdString)
	if err != nil {
		logger.Warn("Invalid query parameter for searching for person", zap.Int("personId", personId), zap.String("Error", err.Error()))
		http.Error(w, "Invalid query parameter 'personId'", http.StatusBadRequest)
		return
	}

	personDetails, err := s.klubadmin.GetPerson(personId)

	if err != nil {
		fmt.Printf("Unable to do GetPerson for %v\nError:%v\n", personId, err)
		http.Error(w, "Internal Server Error doing GetPerson", http.StatusInternalServerError)
		return
	}
	render.JSON(w, request, personDetails)
}

func (s *MemberHandlers) GetSearch(w http.ResponseWriter, request *http.Request) {
	s.logger.Info("GetSearch")
	searchName := request.URL.Query().Get("name")
	if searchName == "" {
		http.Error(w, "Missing query parameter 'name'", http.StatusBadRequest)
		return
	}

	persons, err := s.klubadmin.Search(searchName)

	if err != nil {
		fmt.Printf("Unable to do search for %v\nError:%v\n", searchName, err)
		http.Error(w, "Internal Server Error doing Search", http.StatusInternalServerError)
		return
	}

	err = OkJson(persons, w)
	if err != nil {
		http.Error(w, "Internal Server Error writing response", http.StatusInternalServerError)
		return
	}
}

func (s *MemberHandlers) GetSearchAll(w http.ResponseWriter, request *http.Request) {
	s.logger.Info("GetSearchAll")
	searchName := request.URL.Query().Get("name")
	if searchName == "" {
		http.Error(w, "Missing query parameter 'name'", http.StatusBadRequest)
		return
	}

	persons, err := s.klubadmin.SearchAll(searchName)
	if err != nil {
		fmt.Printf("Unable to do search all for %v\nError:%v\n", searchName, err)
		http.Error(w, "Internal Server Error doing SearchAll", http.StatusInternalServerError)
		return
	}

	err = OkJson(persons, w)
	if err != nil {
		http.Error(w, "Internal Server Error writing response", http.StatusInternalServerError)
		return
	}
}

func OkJson(data any, w http.ResponseWriter) error {
	jsonData, err := json.Marshal(data)
	if err != nil {
		return err
	}

	w.Header().Set("Content-Type", "application/json")
	_, err = w.Write(jsonData)
	if err != nil {
		return err
	}
	return nil
}
