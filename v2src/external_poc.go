// You can edit this code!
// Click here and start typing.
package main

import (
	"VAF13/External/Klubadmin"
	"fmt"
)

func ExternalPocTest() {
	fmt.Println("===========")
	search, err := Klubadmin.Search("Nikolaj")
	if err != nil {
		fmt.Println("Error searching", err)
	} else {
		for i, value := range search {
			fmt.Printf("SearchResult Person[%v]:%+v\n", i, value)
		}
		fmt.Println("===========")
		searchFirst := search[0]
		person, err := Klubadmin.GetPerson(searchFirst)
		fmt.Printf("GetPerson for:%+v\nResult:%+v\nErr:%v\n", searchFirst, person, err)

	}

	fmt.Println("===========")
	persons, err := Klubadmin.SearchAll("Lund")
	for i, value := range persons {
		fmt.Printf("SearchAll Person[%v]:%+v\n", i, value)
	}
	fmt.Println("===========")
	fmt.Println("DONE")

}
