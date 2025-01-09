package Klubadmin

import (
	"VAF13/Helpers"
	"fmt"
	"github.com/antchfx/htmlquery"
	"strconv"
	"strings"
)

type MappingService interface {
	ConvertSearchResultToResponse(person *SearchResultPerson) (*SearchResultResponse, error)
	ConvertHtmlPersonToPerson(personId int, personHtmlDetails string) (*PersonResponse, error)
}

type DefaultMappings struct {
}

func NewDefaultMappingService() MappingService {
	return &DefaultMappings{}
}
func (service *DefaultMappings) ConvertSearchResultToResponse(ptr *SearchResultPerson) (*SearchResultResponse, error) {
	if ptr == nil {
		return nil, fmt.Errorf("unable to convert search result to response")
	}
	mapped := &SearchResultResponse{
		Name:        ptr.Name,
		Club:        ptr.Club,
		DateAdded:   ptr.DateAdded,
		DateRemoved: ptr.DateRemoved,
		Address:     ptr.Address,
		DFUNo:       ptr.DFUNo,
		Birthday:    ptr.Birthday,
		Phone:       ptr.Phone,
		Email:       ptr.Email,
		Type:        ptr.Type,
		Certificate: ptr.Certificate,
		Id:          ptr.Id,
	}

	return mapped, nil
}

func (service *DefaultMappings) ConvertHtmlPersonToPerson(personId int, personHtmlDetails string) (*PersonResponse, error) {
	doc, err := Helpers.GetHtmlNodeFromString(personHtmlDetails)
	if err != nil {
		return nil, fmt.Errorf("unable to parse html to html node")
	}

	var fullName string = Helpers.GetElementAttributeValue(doc, "person_firstname", "value")
	nameSplit := strings.Split(fullName, " ")

	//Certificate
	certificateValue := Helpers.GetElementAttributeValue(doc, "certificateNr_3", "data-orgvalue")
	certificate := 0
	if c, err := strconv.Atoi(certificateValue); err == nil {

		certificate = c
	}

	// Club
	clubText := ""
	clubElement := Helpers.GetElementById(doc, "currentMembershipsTable")
	if clubElement != nil {
		clubNodenode := htmlquery.FindOne(clubElement, "//tbody/tr[1]/td[1]")
		if clubNodenode != nil {
			clubText = htmlquery.InnerText(clubNodenode)
		}
	}

	result := PersonResponse{
		Id:              personId,
		FirstName:       nameSplit[0],
		LastName:        strings.Join(nameSplit[1:], " "),
		Address:         Helpers.GetElementAttributeValue(doc, "person_address", "value"),
		Zip:             Helpers.GetElementAttributeValue(doc, "person_zip", "value"),
		Club:            clubText,
		City:            Helpers.GetElementAttributeValue(doc, "person_city", "value"),
		Country:         Helpers.GetElementAttributeValue(doc, "person_country", "data-orgvalue"),
		Email:           Helpers.GetElementAttributeValue(doc, "person_mail", "value"),
		Phone:           Helpers.GetElementAttributeValue(doc, "person_cellular", "value"),
		Birthday:        Helpers.GetElementAttributeValue(doc, "person_birthdayDate_inverted", "value"),
		ContactName:     Helpers.GetElementAttributeValue(doc, "relative_firstname", "value"),
		ContactPhone:    Helpers.GetElementAttributeValue(doc, "relative_cellular", "value"),
		ContactRelation: Helpers.GetElementAttributeValue(doc, "person_relativerelation", "value"),
		Gender:          Helpers.GetElementAttributeValue(doc, "gender", "data-orgvalue"),
		Certificate:     certificate,
	}
	return &result, nil
}
