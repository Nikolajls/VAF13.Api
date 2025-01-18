using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.DTOs.KlubadminResults;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin;

public class KlubAdminMappingService : IKlubAdminMappingService
{
  private readonly ILogger<KlubAdminMappingService> _logger;
  private readonly string _hiddenField = "(skjult)";

  public KlubAdminMappingService(ILogger<KlubAdminMappingService> logger)
  {
    _logger = logger;
    _logger.LogInformation("KlubAdminMappingService instantiated");
  }

  public PersonDetailsResponse MapPersonDetailsResponseFromHtml(int personId, string responseString)
  {
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

    if (address == _hiddenField)
    {
      address = "";
    }
    if (city == _hiddenField)
    {
      city = "";
    }
    if (zipCode == _hiddenField)
    {
      zipCode = "";
    }
    if (birthday == _hiddenField)
    {
      birthday = "";
    }
    if (phone == _hiddenField)
    {
      phone = "";
    }
    if (mail == _hiddenField)
    {
      mail = "";
    }

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
    return response;
  }

  public PersonSearchResponse MapPersonSearchResponseFromSearch(PersonSearchResult person)
  {
    var fullNameSplit = person.Name.Split(" ");
    var firstName = (fullNameSplit.Length > 0 ? fullNameSplit[0] : string.Empty).Trim();
    var lastName = (fullNameSplit.Length > 1 ? string.Join(" ", fullNameSplit[1..]) : string.Empty).Trim();
    var mapped = new PersonSearchResponse()
    {
      FirstName = firstName,
      LastName = lastName,
      Club = person.Club,
      DateAdded = person.DateAdded,
      DateRemoved = person.DateRemoved,
      Address = person.Address != _hiddenField ? person.Address : "",
      DFUNo = int.TryParse(person.DFUNo, out var dfuNo) ? dfuNo : 0,
      Birthday = person.Birthday != _hiddenField ? person.Birthday : "",
      Phone = person.Phone != _hiddenField ? person.Phone : "",
      Email = person.Email != _hiddenField ? person.Email : "",
      Type = person.Type,
      Certificate = int.TryParse(person.Certificate, out var cert) ? cert : 0,
      Id = int.TryParse(person.Id, out var id) ? id : 0,
    };
    return mapped;
  }
}