namespace VAF13.Klubadmin.API.Services.Interfaces;

public interface IKlubAdminAuthService
{
  Task<string> Authenticate();
}