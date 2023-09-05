using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Web;
using VAF13.Klubadmin.API.Services.Interfaces;
using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.API.Services;


public class KlubAdminService : IKlubAdminService
{
  private readonly ILogger<KlubAdminService> _logger;
  private readonly HttpClient _httpClient;
  public KlubAdminService(HttpClient httpClient, ILogger<KlubAdminService> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
  }

  public async Task<IEnumerable<PersonSearchResult>> SearchPerson(string name)
  {
    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
    int secondsSinceEpoch = (int)t.TotalSeconds;
    var nameEncoded = HttpUtility.UrlEncode(name);

    var queryUrl = $"klubadmin/pages/p_members/server_processing.php?draw=12&start=0&length=10&search%5Bvalue%5D={nameEncoded}&search%5Bregex%5D=false&_t={secondsSinceEpoch}";

    _logger.LogInformation("Searching for name {name}", name);
    var requestResponse = await _httpClient.GetAsync(queryUrl);
    _logger.LogInformation("HTTP Response for search on name: on {name}, {StatusCode}", name, requestResponse.StatusCode);

    requestResponse.EnsureSuccessStatusCode();
    var responseString = await requestResponse.Content.ReadAsStringAsync();

    var searchResult = JsonConvert.DeserializeObject<SearchResult>(responseString);
    if (searchResult is not null)
    {
      return searchResult.data;
    }

    return Enumerable.Empty<PersonSearchResult>();
  }

  public async Task<PersonDetails?> GetPersonInfo(string personId)
  {
    var loginDict = new Dictionary<string, string>()
    {
      ["personId"] = personId,
      ["group"] = "personInfo",
    };

    var personDetails = new FormUrlEncodedContent(loginDict);

    _logger.LogInformation("Requesting info on Profile {UserId}", loginDict["personId"]);
    var requestResponse = await _httpClient.PostAsync("klubadmin/pages/ajax.php", personDetails);
    _logger.LogInformation("HTTP Response for info on {UserId}, {StatusCode}", loginDict["personId"], requestResponse.StatusCode);

    requestResponse.EnsureSuccessStatusCode();
    var responseString = await requestResponse.Content.ReadAsStringAsync();

    var doc = new HtmlDocument();
    doc.LoadHtml(responseString);
    var firstnameValue = doc.GetElementValueById("person_firstname");
    var address = doc.GetElementValueById("person_address");
    var zipCode = doc.GetElementValueById("person_zip");
    var city = doc.GetElementValueById("person_city");
    var country = doc.GetElementValueById("person_country");
    var mail = doc.GetElementValueById("person_mail");
    var phone = doc.GetElementValueById("person_cellular");
    var birthday = doc.GetElementValueById("person_birthdayDate_inverted");
    var iceName = doc.GetElementValueById("relative_firstname");
    var icePhone = doc.GetElementValueById("relative_cellular");
    var iceRelation = doc.GetElementValueById("person_relativerelation");

    return new PersonDetails()
    {
      Name = firstnameValue,
      Address = address,
      Zip = zipCode,
      City = city,
      Country = country,
      Mail = mail,
      Phone = phone,
      Birthday = birthday,
      ContactName = iceName,
      ContactPhone = icePhone,
      ContactRelation = iceRelation
    };
  }
}