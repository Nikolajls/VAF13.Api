﻿using System.Web;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.DTOs.KlubadminResults;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin;

public class KlubAdminService : IKlubAdminService
{
    private readonly ILogger<KlubAdminService> _logger;
    private readonly HttpClient _httpClient;

    public KlubAdminService(HttpClient httpClient, ILogger<KlubAdminService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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
            return Enumerable.Empty<PersonDetailsResponse>();
        }

        var persons = new List<PersonDetailsResponse>();

        var detailsTasks = allSearchResults.Select(async c =>
        {
            var details = await GetPersonInfo(c.Id);
            if (details != null)
            {
                details.Certificate = c.Certificate;
                persons.Add(details);
            }
        }).ToArray();
        await Task.WhenAll(detailsTasks);

        persons = persons
            .OrderBy(x => x.FirstName)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.Club)
            .ToList();
        return persons;
    }

    public async Task<IEnumerable<PersonSearchResponse>> SearchPerson(string name)
    {
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        var nameEncoded = HttpUtility.UrlEncode(name);

        var queryUrl = $"klubadmin/pages/p_members/server_processing.php?draw=12&start=0&length=-1&search%5Bvalue%5D={nameEncoded}&search%5Bregex%5D=false&_t={secondsSinceEpoch}";

        _logger.LogInformation("Searching for name {name}", name);
        string responseString;
        try
        {
            var requestResponse = await _httpClient.GetAsync(queryUrl);
            _logger.LogInformation("HTTP Response for search on name: on {name}, {StatusCode}", name,
                requestResponse.StatusCode);

            requestResponse.EnsureSuccessStatusCode();
            responseString = await requestResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to find data on SearchPerson for {Name} - {Message}", name, ex.Message);
            return Enumerable.Empty<PersonSearchResponse>();
        }

        var searchResult = JsonConvert.DeserializeObject<SearchResult>(responseString);
        if (searchResult is not null)
        {
            return searchResult.data.Select(person =>
            {
                var obj = new PersonSearchResponse(person);
                return obj;
            }).ToList();
        }

        return Enumerable.Empty<PersonSearchResponse>();
    }

    public async Task<PersonDetailsResponse?> GetPersonInfo(int personId)
    {
        var loginDict = new Dictionary<string, string>()
        {
            ["personId"] = personId.ToString(),
            ["group"] = "personInfo",
        };
        var personDetails = new FormUrlEncodedContent(loginDict);

        _logger.LogInformation("Requesting info on Profile {UserId}", loginDict["personId"]);
        string responseString;
        HttpResponseMessage? requestResponse = null;
        try
        {
            requestResponse = await _httpClient.PostAsync("klubadmin/pages/ajax.php", personDetails);
            _logger.LogInformation("HTTP Response for info on {UserId}, {StatusCode}", loginDict["personId"],
                requestResponse.StatusCode);
            requestResponse.EnsureSuccessStatusCode();
            responseString = await requestResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during GetPersonInfo for {PersonId} - {Message} - {StatusCode}", personId, ex.Message, requestResponse?.StatusCode);
            return null;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(responseString);
        var fullName = doc.GetElementValueById("person_firstname");
        var fullNameSplit = fullName.Split(" ");
        var firstnameValue = (fullNameSplit.Length > 0 ? fullNameSplit[0] : string.Empty).Trim();
        var lastNameValue = (fullNameSplit.Length > 1 ? string.Join(" ", fullNameSplit[1..]) : string.Empty).Trim();

        var address = doc.GetElementValueById("person_address");
        var zipCode = doc.GetElementValueById("person_zip");
        var city = doc.GetElementValueById("person_city");
        var country = doc.GetDropdownValue("person_country");
        var mail = doc.GetElementValueById("person_mail");
        var phone = doc.GetElementValueById("person_cellular");
        var birthday = doc.GetElementValueById("person_birthdayDate_inverted");
        var iceName = doc.GetElementValueById("relative_firstname");
        var icePhone = doc.GetElementValueById("relative_cellular");
        var iceRelation = doc.GetElementValueById("person_relativerelation");
        var gender = doc.GetDropdownValue("gender");
        var certificateNr = doc.GetElementValueById("certificateNr_3");
        var clubStr = doc.GetElementbyId("currentMembershipsTable").SelectNodes("//tbody/tr[1]/td[1]").FirstOrDefault()?.InnerText ?? string.Empty;
       
        var response = new PersonDetailsResponse()
        {
            Id = personId,
            FirstName = firstnameValue,
            LastName = lastNameValue,
            Address = address,
            Zip = zipCode,
            Club = clubStr,
            City = city,
            Country = country,
            Email = mail,
            Phone = phone,
            Birthday = birthday,
            ContactName = iceName,
            ContactPhone = icePhone,
            ContactRelation = iceRelation,
            Gender = gender,
            Certificate = int.TryParse(certificateNr, out var cert) ? cert : 0,
        };
        response.Cleanup();
        return response;
    }
}