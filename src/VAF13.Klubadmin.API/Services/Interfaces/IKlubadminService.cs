using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.API.Services.Interfaces
{
  public interface IKlubAdminService
  {
    Task<IEnumerable<PersonSearchResult>> SearchPerson(string name);

    Task<PersonDetails?> GetPersonInfo(string personId);
  }
}
