using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces
{
    public interface IKlubAdminService
    {
        Task<IEnumerable<PersonSearchResult>> SearchPerson(string name);
        Task<IEnumerable<PersonDetails>> SearchAll(string name);

        Task<PersonDetails?> GetPersonInfo(string personId, string club);
    }
}
