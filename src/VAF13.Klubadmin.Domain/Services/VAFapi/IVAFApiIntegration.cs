using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.Domain.Services.VAFapi
{
    public interface IVafApiIntegration
    {
        Task<List<PersonDetails>> SearchAll(string name);
    }
}
