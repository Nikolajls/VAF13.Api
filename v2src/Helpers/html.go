package Helpers

import (
	"fmt"
	"golang.org/x/net/html"
	"strings"
)

func GetAttribute(n *html.Node, key string) (string, bool) {
	for _, attr := range n.Attr {
		if attr.Key == key {
			return attr.Val, true
		}
	}
	return "", false
}

func checkId(n *html.Node, id string) bool {
	if n.Type == html.ElementNode {
		s, ok := GetAttribute(n, "id")
		if ok && s == id {
			return true
		}
	}
	return false
}

func traverse(n *html.Node, id string) *html.Node {
	if checkId(n, id) {
		return n
	}

	for c := n.FirstChild; c != nil; c = c.NextSibling {
		result := traverse(c, id)
		if result != nil {
			return result
		}
	}

	return nil
}

func GetElementById(n *html.Node, id string) *html.Node {
	return traverse(n, id)
}

func GetElementAttributeValue(n *html.Node, id string, key string) string {
	element := GetElementById(n, id)
	if element == nil {
		return ""
	}
	attributeValue, _ := GetAttribute(element, key)

	return attributeValue
}

func GetHtmlNodeFromString(htmlString string) (*html.Node, error) {
	if htmlString == "" {
		return nil, fmt.Errorf("htmlString is empty unable to parse to htmlnode")
	}
	doc, err := html.Parse(strings.NewReader(htmlString))
	return doc, err
}
