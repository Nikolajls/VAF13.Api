using Microsoft.Extensions.Logging;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.Domain.Infrastructure;

public class KlubadminAuthHandler : DelegatingHandler
{
    private readonly ILogger<KlubadminAuthHandler> _logger;
    private readonly IKlubAdminAuthService _authService;

    public KlubadminAuthHandler(ILogger<KlubadminAuthHandler> logger, IKlubAdminAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    private async Task ModifyRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authId = await _authService.Authenticate();
        if (string.IsNullOrEmpty(authId))
        {
            throw new ArgumentNullException(nameof(authId));
        }

        request.Headers.Add("Cookie", $"PHPSESSID={authId}");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await ModifyRequest(request, cancellationToken).ConfigureAwait(false);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}