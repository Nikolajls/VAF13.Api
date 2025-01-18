using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.Domain.Services.VAFapi
{
  public interface IVafApiIntegration
  {
    Task<List<PersonDetailsResponse>> SearchAll(string name);
  }
}
