using Newtonsoft.Json;
using VAF13.Klubadmin.Domain.DTOs.KlubadminResults;

namespace VAF13.Klubadmin.Domain.DTOs;

public record PersonSearchResponse
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Club { get; set; } = string.Empty;
  public string DateAdded { get; set; } = string.Empty;
  public string DateRemoved { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
  public int DFUNo { get; set; }
  public string Birthday { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public int Certificate { get; set; }
  public int Id { get; set; }
}
