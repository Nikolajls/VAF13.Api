namespace VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

public interface IKlubAdminAuthService
{
    Task<string> Authenticate();
}