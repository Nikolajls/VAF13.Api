package Klubadmin

type LoginResult struct {
	Result int    `json:"result"`
	Type   string `json:"type"`
}

type SearchResult struct {
	Draw            int                  `json:"draw"`
	RecordsTotal    int                  `json:"recordsTotal"`
	RecordsFiltered int                  `json:"recordsFiltered"`
	Data            []SearchResultPerson `json:"data"`
}

type SearchResultPerson struct {
	DTRowClass  string `json:"DT_RowClass"`
	Name        string `json:"0"`
	Club        string `json:"1"`
	DateAdded   string `json:"2"`
	DateRemoved string `json:"3"`
	Address     string `json:"4"`
	DFUNo       int    `json:"5"`
	Birthday    string `json:"6"`
	Phone       string `json:"7"`
	Email       string `json:"8"`
	Type        string `json:"9"`
	Certificate int    `json:"10"`
	Id          int    `json:"11"`
}

type SearchResultResponse struct {
	Name        string `json:"Name"`
	Club        string `json:"Club"`
	DateAdded   string `json:"DateAdded"`
	DateRemoved string `json:"DateRemoved"`
	Address     string `json:"Address"`
	DFUNo       int    `json:"DFUNo"`
	Birthday    string `json:"Birthday"`
	Phone       string `json:"Phone"`
	Email       string `json:"Email"`
	Type        string `json:"Type"`
	Certificate int    `json:"Certificate"`
	Id          int    `json:"Id"`
}

type PersonResponse struct {
	Id              string `json:"Id"`
	FirstName       string `json:"FirstName"`
	LastName        string `json:"LastName"`
	Address         string `json:"Address"`
	City            string `json:"City"`
	Zip             string `json:"Zip"`
	Country         string `json:"Country"`
	Mail            string `json:"Mail"`
	Phone           string `json:"Phone"`
	Birthday        string `json:"Birthday"`
	Club            string `json:"Club"`
	ContactRelation string `json:"ContactRelation"`
	ContactName     string `json:"ContactName"`
	ContactPhone    string `json:"ContactPhone"`
	Gender          string `json:"Gender"`
	Certificate     string `json:"Certificate"`
}

type SearchResultCleanUp interface {
	CleanupResult()
}

func (result *SearchResultPerson) CleanupResult() {
	if result.Address == "(skjult)" {
		result.Address = ""
	}
	if result.Birthday == "(skjult)" {
		result.Birthday = ""
	}
	if result.Phone == "(skjult)" {
		result.Phone = ""
	}
	if result.Email == "(skjult)" {
		result.Email = ""
	}
}

func (result *PersonResponse) CleanupResult() {
	if result.Address == "(skjult)" {
		result.Address = ""
	}
	if result.City == "(skjult)" {
		result.City = ""
	}
	if result.Zip == "(skjult)" {
		result.Zip = ""
	}
	if result.Mail == "(skjult)" {
		result.Mail = ""
	}
	if result.Phone == "(skjult)" {
		result.Phone = ""
	}
	if result.Birthday == "(skjult)" {
		result.Birthday = ""
	}
}
