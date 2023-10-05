using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VAF13.Klubadmin.Domain.Configurations;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin;

public class KlubAdminAuthService : IKlubAdminAuthService
{
    private readonly ILogger<KlubAdminAuthService> _logger;
    private readonly DFUConfiguration _apiDfuConfiguraton;
    private readonly HttpClient _httpClient;

    public KlubAdminAuthService(HttpClient httpClient, ILogger<KlubAdminAuthService> logger, IOptions<DFUConfiguration> apiDfuConfiguraton)
    {
        _httpClient = httpClient;
        _apiDfuConfiguraton = apiDfuConfiguraton.Value;
        _logger = logger;
    }

    public async Task<string> Authenticate()
    {
        var loginDict = new Dictionary<string, string>()
        {
            ["loginid"] = _apiDfuConfiguraton.Username,
            ["password"] = _apiDfuConfiguraton.Password,
            ["action"] = "systemlogin",
        };

        var loginContentTwo = new FormUrlEncodedContent(loginDict);
        _logger.LogInformation("getting login access");
        var loginResponse = await _httpClient.PostAsync("klubadmin/pages/", loginContentTwo);
        _logger.LogInformation("Got Login Access {StatusCode}", loginResponse.StatusCode);

        var responseString = await loginResponse.Content.ReadAsStringAsync();

        loginResponse.EnsureSuccessStatusCode();
        var cookieValue = string.Empty;
        var cookieSetHeader = loginResponse.Headers.Where(c => c.Key == "Set-Cookie").SelectMany(c => c.Value).FirstOrDefault(e => e.Contains("PHPSESSID"));
        if (!string.IsNullOrEmpty(cookieSetHeader))
        {
            cookieValue = cookieSetHeader.Remove(cookieSetHeader.IndexOf(";")).Replace("PHPSESSID=", "");
        }

        return cookieValue;
    }
}