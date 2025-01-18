using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

public interface IKlubAdminService
{
  Task<IEnumerable<PersonSearchResponse>> SearchPerson(string name);

  Task<IEnumerable<PersonDetailsResponse>> SearchAll(string name);

  Task<PersonDetailsResponse?> GetPersonInfo(int personId);
}
