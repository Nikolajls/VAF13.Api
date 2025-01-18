using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.DTOs.KlubadminResults;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

public interface IKlubAdminMappingService
{
  PersonDetailsResponse MapPersonDetailsResponseFromHtml(int personId, string responseString);

  PersonSearchResponse MapPersonSearchResponseFromSearch(PersonSearchResult person);
}
