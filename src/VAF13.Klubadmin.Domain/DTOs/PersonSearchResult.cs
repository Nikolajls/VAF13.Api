using Newtonsoft.Json;

namespace VAF13.Klubadmin.Domain.DTOs;

public class PersonSearchResult
{
  public string DT_RowClass { get; set; }
  [JsonProperty("0")] public string Name { get; set; }
  [JsonProperty("1")] public string Club { get; set; }
  [JsonProperty("2")] public string DateAdded { get; set; }
  [JsonProperty("3")] public string DateRemoved { get; set; }
  [JsonProperty("4")] public string Address { get; set; }
  [JsonProperty("5")] public string DFUNo { get; set; }
  [JsonProperty("6")] public string Birthday { get; set; }
  [JsonProperty("7")] public string Phone { get; set; }
  [JsonProperty("8")] public string Email { get; set; }
  [JsonProperty("9")] public string Id { get; set; }
}