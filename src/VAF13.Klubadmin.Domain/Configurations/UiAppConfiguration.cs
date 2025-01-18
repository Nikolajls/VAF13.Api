namespace VAF13.Klubadmin.Domain.Configurations;

public class UiAppConfiguration
{
  public const string ConfigurationSectionName = "UIConfiguration";
  public string ApiEndpoint { get; set; } = string.Empty;
  public string APIKey { get; set; } = string.Empty;
}
