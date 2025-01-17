﻿namespace VAF13.Klubadmin.Domain.Configurations;

public class ApiConfiguration
{
  public const string ConfigurationSectionName = "ApiConfiguration";
  public string APIKey { get; set; } = string.Empty;
  public string CorsOrigins { get; set; } = string.Empty;
  public bool AddCors => !string.IsNullOrEmpty(CorsOrigins);
}
