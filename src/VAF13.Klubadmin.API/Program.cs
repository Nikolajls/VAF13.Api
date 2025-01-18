using Microsoft.OpenApi.Models;
using Serilog;
using System.Net.Http.Headers;
using VAF13.Klubadmin.API.Infrastucture;
using VAF13.Klubadmin.Domain.Configurations;
using VAF13.Klubadmin.Domain.Infrastructure;
using VAF13.Klubadmin.Domain.Services.Klubadmin;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.API;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, config) =>
    {
      config.WriteTo.Console();
    });

    //builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

    var services = builder.Services;
    ConfigureServices(services, builder.Configuration);

    //
    var app = builder.Build();
    ConfigureApp(app);

    app.Run();
  }

  private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
  {
    services.AddControllers();
    services.AddEndpointsApiExplorer();
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

    services.AddOptions();
    services.Configure<ApiConfiguration>(configuration.GetSection(ApiConfiguration.ConfigurationSectionName));
    services.Configure<DFUConfiguration>(configuration.GetSection(DFUConfiguration.ConfigurationSectionName));

    //Add authentication
    services.AddHttpClient<IKlubAdminAuthService, KlubAdminAuthService>(client =>
    {
      client.BaseAddress = new Uri("https://klubadmin.dfu.dk/");
      client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.23"));
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
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.23"));
      })
      .AddHttpMessageHandler<KlubadminAuthHandler>()
      .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
          UseCookies = false
        }
      );
  }

  private static void ConfigureApp(WebApplication app)
  {
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseMiddleware<ApiKeyMiddleware>();

    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
  }
}
