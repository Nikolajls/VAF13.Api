package handlers

import (
	"VAF13/External/Klubadmin"
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
)

func GetPerson(w http.ResponseWriter, request *http.Request) {
	personIdString := request.URL.Query().Get("personId")
	club := request.URL.Query().Get("club")
	if personIdString == "" {
		http.Error(w, "Missing query parameter 'personId'", http.StatusBadRequest)
		return
	}

	personId, err := strconv.Atoi(personIdString)
	if err != nil {
		http.Error(w, "Invalid query parameter 'personId'", http.StatusBadRequest)
		return
	}

	personDetails, err := Klubadmin.GetPerson(personId, club)

	if err != nil {
		fmt.Printf("Unable to do GetPerson for %v\nError:%v\n", personId, err)
		http.Error(w, "Internal Server Error doing GetPerson", http.StatusInternalServerError)
		return
	}

	err = OkJson(personDetails, w)
	if err != nil {
		http.Error(w, "Internal Server Error writing response", http.StatusInternalServerError)
		return
	}
}

func GetSearch(w http.ResponseWriter, request *http.Request) {
	searchName := request.URL.Query().Get("name")
	if searchName == "" {
		http.Error(w, "Missing query parameter 'name'", http.StatusBadRequest)
		return
	}

	persons, err := Klubadmin.Search(searchName)

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

func GetSearchAll(w http.ResponseWriter, request *http.Request) {
	searchName := request.URL.Query().Get("name")
	if searchName == "" {
		http.Error(w, "Missing query parameter 'name'", http.StatusBadRequest)
		return
	}

	persons, err := Klubadmin.SearchAll(searchName)
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
