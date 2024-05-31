using Newtonsoft.Json;

namespace VAF13.Klubadmin.Domain.DTOs;

public class PersonSearchResult
{
    public string DT_RowClass { get; set; } = string.Empty;
    [JsonProperty("0")] public string Name { get; set; } = string.Empty;
    [JsonProperty("1")] public string Club { get; set; } = string.Empty;
    [JsonProperty("2")] public string DateAdded { get; set; } = string.Empty;
    [JsonProperty("3")] public string DateRemoved { get; set; } = string.Empty;
    [JsonProperty("4")] public string Address { get; set; } = string.Empty;
    [JsonProperty("5")] public string DFUNo { get; set; } = string.Empty;
    [JsonProperty("6")] public string Birthday { get; set; } = string.Empty;
    [JsonProperty("7")] public string Phone { get; set; } = string.Empty;
    [JsonProperty("8")] public string Email { get; set; } = string.Empty;
    [JsonProperty("9")] public string Type { get; set; } = string.Empty;
    [JsonProperty("10")] public string Certificate { get; set; } = string.Empty;
    [JsonProperty("11")] public string Id { get; set; } = string.Empty;
}