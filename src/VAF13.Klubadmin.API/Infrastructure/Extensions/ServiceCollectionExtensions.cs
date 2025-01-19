using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using VAF13.Klubadmin.Domain.Configurations;
using VAF13.Klubadmin.Domain.Infrastructure;
using VAF13.Klubadmin.Domain.Services.Klubadmin;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
  public static void AddSwaggerAPIKeyScheme(this IServiceCollection services)
  {
    services.AddSwaggerGen(x =>
    {
      x.AddSecurityDefinition("X-API-KEY", new OpenApiSecurityScheme
      {
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        In = ParameterLocation.Header,
        Description = "ApiKey must appear in header"
      });
      x.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = "X-API-KEY"
            },
            In = ParameterLocation.Header
          },
          new string[]{}
        }
      });
    });
  }

  public static void AddKlubAdmin(this IServiceCollection services)
  {
    //Add authentication klubadmin service
    services.AddHttpClient<IKlubAdminAuthService, KlubAdminAuthService>(client =>
      {
        client.BaseAddress = new Uri("https://klubadmin.dfu.dk/");
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VAFAPI", "1.0"));
      })
      .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
          UseCookies = false
        }
      );

    services.AddSingleton<IKlubAdminMappingService, KlubAdminMappingService>();

    services.AddTransient<KlubadminAuthHandler>();
    services.AddHttpClient<IKlubAdminService, KlubAdminService>(client =>
      {
        client.BaseAddress = new Uri("https://klubadmin.dfu.dk/");
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VAFAPI", "1.0"));
      })
      .AddHttpMessageHandler<KlubadminAuthHandler>()
      .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
          UseCookies = false
        }
    );
  }

  public static (ApiConfiguration apiConfiguration, DFUConfiguration dfu) AddConfigurationOptions(this IServiceCollection services, ConfigurationManager configuration)
  {
    services.AddOptions();
    services.Configure<ApiConfiguration>(configuration.GetSection(ApiConfiguration.ConfigurationSectionName));
    services.Configure<DFUConfiguration>(configuration.GetSection(DFUConfiguration.ConfigurationSectionName));

    using var tempServiceProvider = services.BuildServiceProvider();
    var apiOptions = tempServiceProvider.GetRequiredService<IOptions<ApiConfiguration>>().Value;
    var dfuOptions = tempServiceProvider.GetRequiredService<IOptions<DFUConfiguration>>().Value;

    return (apiOptions, dfuOptions);
  }
}
