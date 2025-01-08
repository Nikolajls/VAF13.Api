package Helpers

import (
	"fmt"
	"github.com/stretchr/testify/assert"
	"testing"
)

func wrapHtmlInDiv(innerHtml string) string {
	return "<div>" + innerHtml + "</div>"
}

func Test_GetElementById(t *testing.T) {
	arrangedHtml := wrapHtmlInDiv(`<input name="mail"  id="person_mail" data-orgvalue="testemail@tld.com"  value="testemail@tld.com">`)
	htmlNode, _ := GetHtmlNodeFromString(arrangedHtml)

	tests := []struct {
		name        string
		id          string
		returnsNode bool
	}{
		{name: "Success", id: "person_mail", returnsNode: true},
		{name: "Fails due to missing Id", id: "person_mail_nope", returnsNode: false},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got := GetElementById(htmlNode, tt.id)
			returnedNode := got != nil
			assert.Equal(t, tt.returnsNode, returnedNode, fmt.Sprintf("GetElementById(%v) returns a node= %v, wanted %v", tt.id, returnedNode, tt.returnsNode))

		})
	}
}

func Test_GetElementAttributeValue(t *testing.T) {
	arrangedEmail := "testemail@tld.com"
	arrangedAttribute := "data-orgvalue"
	elementStr := fmt.Sprintf(`<input name="mail"  id="person_mail" %v="%v"  value="%v">`, arrangedAttribute, arrangedEmail, arrangedEmail)
	arrangedHtml := wrapHtmlInDiv(elementStr)
	htmlNode, _ := GetHtmlNodeFromString(arrangedHtml)

	tests := []struct {
		name          string
		id            string
		attribute     string
		expectedValue string
	}{
		{name: "Success", id: "person_mail", attribute: "data-orgvalue", expectedValue: arrangedEmail},
		{name: "Fails due to element with id", id: "person_mail_fake", attribute: "data-org-value", expectedValue: ""},
		{name: "Fails due to missing attribute", id: "person_mail", attribute: "data-orgvalue_fake", expectedValue: ""},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got := GetElementAttributeValue(htmlNode, tt.id, tt.attribute)
			assert.Equal(t, tt.expectedValue, got, fmt.Sprintf("GetElementAttributeValue(%v, %v) returns = %v, wanted %v", tt.id, tt.attribute, tt.expectedValue, got))
		})
	}
}

func Test_GetHtmlNodeFromString(t *testing.T) {
	type args struct {
		htmlString string
	}
	tests := []struct {
		name        string
		args        args
		returnsNode bool
		wantErr     assert.ErrorAssertionFunc
	}{
		{
			name: "Success",
			args: args{
				htmlString: "<div></div>",
			},
			returnsNode: true,
			wantErr:     assert.NoError,
		}, {
			name: "Fail",
			args: args{
				htmlString: "",
			},
			returnsNode: false,
			wantErr:     assert.Error,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got, err := GetHtmlNodeFromString(tt.args.htmlString)
			if !tt.wantErr(t, err, fmt.Sprintf("GetHtmlNodeFromString(%v)", tt.args.htmlString)) {
				return
			}
			if tt.returnsNode {
				assert.NotNil(t, got, fmt.Sprintf("GetHtmlNodeFromString(%v)", tt.args.htmlString))
			} else {
				assert.Nil(t, got, fmt.Sprintf("GetHtmlNodeFromString(%v)", tt.args.htmlString))
			}
		})
	}
}
