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

type PersonResult struct {
	Id              string `json:"id"`
	FirstName       string `json:"firstName"`
	LastName        string `json:"lastName"`
	Address         string `json:"address"`
	City            string `json:"city"`
	Zip             string `json:"zip"`
	Country         string `json:"country"`
	Mail            string `json:"mail"`
	Phone           string `json:"phone"`
	Birthday        string `json:"birthday"`
	Club            string `json:"club"`
	ContactRelation string `json:"contactRelation"`
	ContactName     string `json:"contactName"`
	ContactPhone    string `json:"contactPhone"`
	Gender          string `json:"gender"`
	Certificate     string `json:"certificate"`
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

func (result *PersonResult) CleanupResult() {
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
