using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Web;
using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.DTOs.KlubadminResults;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin;

public class KlubAdminService : IKlubAdminService
{
  private readonly IKlubAdminMappingService _klubAdminMappingService;
  private readonly ILogger<KlubAdminService> _logger;
  private readonly HttpClient _httpClient;

  public KlubAdminService(HttpClient httpClient, ILogger<KlubAdminService> logger, IKlubAdminMappingService klubAdminMappingService)
  {
    _httpClient = httpClient;
    _logger = logger;
    _klubAdminMappingService = klubAdminMappingService;
    _logger.LogInformation("KlubAdminService instantiated");
  }

  public async Task<IEnumerable<PersonDetailsResponse>> SearchAll(string name)
  {
    IEnumerable<PersonSearchResponse> allSearchResults;
    try
    {
      allSearchResults = await SearchPerson(name);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unable to do Search Person from Search all - {Message}", ex.Message);
      return [];
    }

    ConcurrentBag<PersonDetailsResponse> personsData = new();
    var personDetailsTasks = allSearchResults.Select(async c =>
    {
      var details = await GetPersonInfo(c.Id);
      if (details != null)
      {
        personsData.Add(details);
      }
    }).ToArray();

    await Task.WhenAll(personDetailsTasks);

    List<PersonDetailsResponse> persons = personsData
        .OrderBy(x => x.FirstName)
        .ThenBy(c => c.LastName)
        .ThenBy(c => c.Club)
        .ToList();

    _logger.LogInformation("SearchAll has {Count} entries", persons.Count);
    return persons;
  }

  public async Task<IEnumerable<PersonSearchResponse>> SearchPerson(string name)
  {
    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
    int secondsSinceEpoch = (int)t.TotalSeconds;
    var nameEncoded = HttpUtility.UrlEncode(name);

    var queryUrl = $"klubadmin/pages/p_members/server_processing.php?draw=12&start=0&length=-1&search%5Bvalue%5D={nameEncoded}&search%5Bregex%5D=false&_t={secondsSinceEpoch}";

    _logger.LogInformation("Searching for name {Name}", name);
    string responseString;
    try
    {
      var requestResponse = await _httpClient.GetAsync(queryUrl);
      _logger.LogInformation("HTTP Response for search on name: on {Name}, {StatusCode}", name,
          requestResponse.StatusCode);

      requestResponse.EnsureSuccessStatusCode();
      responseString = await requestResponse.Content.ReadAsStringAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unable to find data on SearchPerson for {Name} - {Message}", name, ex.Message);
      return [];
    }

    var searchResult = JsonConvert.DeserializeObject<SearchResult>(responseString);
    if (searchResult is null)
    {
      return [];
    }

    var resultData = searchResult.data.Select(person =>
    {
      var obj = _klubAdminMappingService.MapPersonSearchResponseFromSearch(person);
      return obj;
    }).ToList();

    _logger.LogInformation("SearchResult has {Count} entries", resultData.Count);
    return resultData;
  }

  public async Task<PersonDetailsResponse?> GetPersonInfo(int personId)
  {
    var loginDict = new Dictionary<string, string>()
    {
      ["personId"] = personId.ToString(),
      ["group"] = "personInfo",
    };
    var personDetails = new FormUrlEncodedContent(loginDict);

    _logger.LogInformation("Requesting info on Member with {PersonId}", personId);
    string responseString;
    HttpResponseMessage? requestResponse = null;
    try
    {
      requestResponse = await _httpClient.PostAsync("klubadmin/pages/ajax.php", personDetails);
      _logger.LogInformation("HTTP Response for info on {PersonId}, {StatusCode}", personId,
          requestResponse.StatusCode);
      requestResponse.EnsureSuccessStatusCode();
      responseString = await requestResponse.Content.ReadAsStringAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception during GetPersonInfo for {PersonId} - {Message} - {StatusCode}", personId, ex.Message, requestResponse?.StatusCode);
      return null;
    }

    var response = _klubAdminMappingService.MapPersonDetailsResponseFromHtml(personId, responseString);
    _logger.LogInformation("Requested and mapped info for PersonId:{PersonId} to {FirstName} {LastName}", personId, response.FirstName, response.LastName);
    return response;
  }
}