using Microsoft.Extensions.Options;
using VAF13.Klubadmin.Domain.Configurations;

namespace VAF13.Klubadmin.API.Infrastructure.Middleware;

public class ApiKeyMiddleware
{
  private readonly RequestDelegate _requestDelegate;
  private readonly ApiConfiguration _apiConfiguration;
  private const string ApiKey = "X-API-KEY";

  public ApiKeyMiddleware(RequestDelegate requestDelegate, IOptions<ApiConfiguration> options)
  {
    _requestDelegate = requestDelegate;
    _apiConfiguration = options.Value;
    if (string.IsNullOrEmpty(_apiConfiguration.APIKey))
    {
      throw new ArgumentException("API Configuration for API key is empty!");
    }
  }

  public async Task Invoke(HttpContext context)
  {
    if (!context.Request.Headers.TryGetValue(ApiKey, out var apiKeyVal))
    {
      context.Response.StatusCode = 401;
      await context.Response.WriteAsync("Api Key not found!");
      return;
    }

    if (!_apiConfiguration.APIKey.Equals(apiKeyVal))
    {
      context.Response.StatusCode = 401;
      await context.Response.WriteAsync("Unauthorized client");
      return;
    }

    await _requestDelegate(context);
  }
}