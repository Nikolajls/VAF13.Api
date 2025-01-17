using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VAF13.Klubadmin.Domain.Configurations;
using VAF13.Klubadmin.Domain.DTOs.KlubadminResults;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin;

public class KlubAdminAuthService : IKlubAdminAuthService
{
  private readonly ILogger<KlubAdminAuthService> _logger;
  private readonly DFUConfiguration _apiDfuConfiguraton;
  private readonly HttpClient _httpClient;
  private string cookieValue = string.Empty;

  public KlubAdminAuthService(HttpClient httpClient, ILogger<KlubAdminAuthService> logger, IOptions<DFUConfiguration> apiDfuConfiguraton)
  {
    _httpClient = httpClient;
    _apiDfuConfiguraton = apiDfuConfiguraton.Value;
    _logger = logger;
    _logger.LogInformation("KlubAdminAuthService instantiated");
  }

  public async Task<string> Authenticate()
  {
    if (!string.IsNullOrEmpty(cookieValue))
    {
      _logger.LogInformation("Already logged in");
      return cookieValue;
    }

    var loginDict = new Dictionary<string, string>()
    {
      ["loginid"] = _apiDfuConfiguraton.Username,
      ["password"] = _apiDfuConfiguraton.Password,
      ["action"] = "systemlogin",
    };

    var loginContentTwo = new FormUrlEncodedContent(loginDict);
    _logger.LogInformation("Doing Klubadmin authentication");
    var loginResponse = await _httpClient.PostAsync("klubadmin/pages/", loginContentTwo);
    _logger.LogInformation("Done with Klubadmin Authentication, Http Status Code: {StatusCode}", loginResponse.StatusCode);

    loginResponse.EnsureSuccessStatusCode();

    var responseString = await loginResponse.Content.ReadAsStringAsync();
    var loginResult = JsonConvert.DeserializeObject<LoginActuionResult>(responseString);
    if (loginResult?.Result <= 0)
    {
      _logger.LogError("Unable to login into Klubadmin result:{Result}", loginResult.Result);
    }

    cookieValue = string.Empty;
    var cookieSetHeader = loginResponse.Headers.Where(c => c.Key == "Set-Cookie").SelectMany(c => c.Value).FirstOrDefault(e => e.Contains("PHPSESSID"));
    if (!string.IsNullOrEmpty(cookieSetHeader))
    {
      cookieValue = cookieSetHeader.Remove(cookieSetHeader.IndexOf(";")).Replace("PHPSESSID=", "");
    }

    return cookieValue;
  }
}