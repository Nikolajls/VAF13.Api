package handlers

import (
	"VAF13/External/Klubadmin"
	"encoding/json"
	"fmt"
	"net/http"
)

func Index(w http.ResponseWriter, request *http.Request) {
	persons, err := Klubadmin.SearchAll("Lund")
	for i, value := range persons {
		fmt.Printf("SearchAll Person[%v]:%+v\n", i, value)
	}
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte("Failed to retrieve"))
		return
	}
	// Set the response header to indicate JSON content
	w.Header().Set("Content-Type", "application/json")

	// Convert the slice to JSON
	jsonData, err := json.Marshal(persons)
	if err != nil {
		http.Error(w, "Error encoding JSON", http.StatusInternalServerError)
		return
	}

	// Write the JSON data to the response
	w.Write(jsonData)
}
