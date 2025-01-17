package Klubadmin

import (
	"fmt"
	"github.com/stretchr/testify/assert"
	"testing"
)

func Test_ConvertHtmlPersonToPerson(t *testing.T) {
	type args struct {
		personId          int
		personHtmlDetails string
	}

	expectedSuccessPerson := PersonResponse{
		Id:              1,
		FirstName:       "Nikolaj",
		LastName:        "Sorensen",
		Address:         "Skydive Lane 123",
		City:            "SkydiveCity",
		Zip:             "666",
		Country:         "Denmark",
		Email:           "testmail@tld.com",
		Phone:           "444",
		Birthday:        "01-01-2000",
		Club:            "Club DFU",
		ContactRelation: "ICE relation",
		ContactName:     "ICE Name",
		ContactPhone:    "ICE PHONE",
		Gender:          "M",
		Certificate:     2866,
	}
	expectedSuccessPersonHtml := fmt.Sprintf(`
<div>
	<input type="text" id="person_firstname" value="%v %v" />
	<input type="text" id="certificateNr_3" data-orgvalue="%v" />
	<table id="currentMembershipsTable">
		<tbody>
			<tr>
				<td>%v</td>
			</tr>
		</tbody>
	</table>
	<input type="text" id="person_address" value="%v" />
	<input type="text" id="person_zip" value="%v" />
	<input type="text" id="person_city" value="%v" />
	<input type="text" id="person_country" data-orgvalue="%v" />
	<input type="text" id="person_mail" value="%v" />
	<input type="text" id="person_cellular" value="%v" />
	<input type="text" id="person_birthdayDate_inverted" value="%v" />
	<input type="text" id="relative_firstname" value="%v" />
	<input type="text" id="relative_cellular" value="%v" />
	<input type="text" id="person_relativerelation" value="%v" />
	<input type="text" id="gender" data-orgvalue="%v" />
</div>`,
		expectedSuccessPerson.FirstName,
		expectedSuccessPerson.LastName,
		expectedSuccessPerson.Certificate,
		expectedSuccessPerson.Club,
		expectedSuccessPerson.Address,
		expectedSuccessPerson.Zip,
		expectedSuccessPerson.City,
		expectedSuccessPerson.Country,
		expectedSuccessPerson.Email,
		expectedSuccessPerson.Phone,
		expectedSuccessPerson.Birthday,
		expectedSuccessPerson.ContactName,
		expectedSuccessPerson.ContactPhone,
		expectedSuccessPerson.ContactRelation,
		expectedSuccessPerson.Gender,
	)

	tests := []struct {
		name    string
		args    args
		want    *PersonResponse
		wantErr bool
	}{
		{
			name: "success",
			args: args{
				personId:          expectedSuccessPerson.Id,
				personHtmlDetails: expectedSuccessPersonHtml,
			},
			want:    &expectedSuccessPerson,
			wantErr: false,
		},
		{
			name: "Fails",
			args: args{
				personId:          666,
				personHtmlDetails: "",
			},
			want:    nil,
			wantErr: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			service := &DefaultMappings{}
			got, err := service.ConvertHtmlPersonToPerson(tt.args.personId, tt.args.personHtmlDetails)
			if (err != nil) != tt.wantErr {
				t.Errorf("convertHtmlPersonToPerson() error = %v, wantErr %v", err, tt.wantErr)
				return
			}
			assert.EqualValues(t, tt.want, got)

		})
	}
}

func TestDefaultMappings_ConvertSearchResultToResponse(t *testing.T) {

	successPerson := &SearchResultPerson{
		DTRowClass:  "DTROW",
		Name:        "Name",
		Club:        "Club",
		DateAdded:   "01-01-2000",
		DateRemoved: "01-02-2000",
		Address:     "Skydive Lane 123",
		DFUNo:       3,
		Birthday:    "01-01-1992",
		Phone:       "404",
		Email:       "404@tld.com",
		Type:        "Skydiver",
		Certificate: 2,
		Id:          1,
	}
	type args struct {
		ptr *SearchResultPerson
	}
	tests := []struct {
		name    string
		args    args
		want    *SearchResultResponse
		wantErr bool
	}{
		{
			name: "Success",
			args: args{
				ptr: successPerson,
			},
			want: &SearchResultResponse{
				Name:        successPerson.Name,
				Club:        successPerson.Club,
				DateAdded:   successPerson.DateAdded,
				DateRemoved: successPerson.DateRemoved,
				Address:     successPerson.Address,
				DFUNo:       successPerson.DFUNo,
				Birthday:    successPerson.Birthday,
				Phone:       successPerson.Phone,
				Email:       successPerson.Email,
				Type:        successPerson.Type,
				Certificate: successPerson.Certificate,
				Id:          successPerson.Id,
			},
			wantErr: false,
		}, {
			name: "Fail due to nil",
			args: args{
				ptr: nil,
			},
			want:    nil,
			wantErr: true,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			service := &DefaultMappings{}
			mappedPerson, err := service.ConvertSearchResultToResponse(tt.args.ptr)
			if (err != nil) != tt.wantErr {
				t.Errorf("ConvertHtmlPersonToPerson() error = %v, wantErr %v", err, tt.wantErr)
				return
			}
			assert.EqualValues(t, tt.want, mappedPerson)
		})
	}
}
