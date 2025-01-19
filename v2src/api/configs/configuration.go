package configs

import (
	"os"
	"strings"
)

type OsConfiguration struct {
	apiKey  string
	origins []string
}

func NewDefaultConfiguration() *OsConfiguration {
	osConfiguration := OsConfiguration{
		apiKey:  os.Getenv("ApiConfiguration__ApiKey"),
		origins: strings.Split(os.Getenv("ApiConfiguration__CorsOrigins"), ","),
	}
	return &osConfiguration
}

type ConfigurationProvider interface {
	ApiKey() string
	AddCors() bool
	CorsOrigins() []string
}

func (conf *OsConfiguration) ApiKey() string {
	return conf.apiKey
}
func (conf *OsConfiguration) AddCors() bool {
	return len(conf.origins) > 0
}

func (conf *OsConfiguration) CorsOrigins() []string {
	return conf.origins
}
