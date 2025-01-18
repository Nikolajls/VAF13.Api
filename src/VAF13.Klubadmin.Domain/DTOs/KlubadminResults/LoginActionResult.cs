namespace VAF13.Klubadmin.Domain.DTOs.KlubadminResults;

public record LoginActuionResult
{
  public int Result { get; set; } = 0;
  public string Type { get; set; } = string.Empty;
}