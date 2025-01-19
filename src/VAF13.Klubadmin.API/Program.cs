using Serilog;
using VAF13.Klubadmin.API.Infrastructure.Extensions;
using VAF13.Klubadmin.API.Infrastructure.Middleware;
using VAF13.Klubadmin.Domain.Configurations;

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
    var apiConfiguration = ConfigureServices(services, builder.Configuration);

    //
    var app = builder.Build();
    ConfigureApp(app, apiConfiguration);

    app.Run();
  }

  private static ApiConfiguration ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
  {
    var (apiConfiguration, _) = services.AddConfigurationOptions(configuration);
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerAPIKeyScheme();
    services.AddKlubAdmin();

    if (apiConfiguration.AddCors)
    {
      services.AddCors(options => options.AddDefaultPolicy(
        policy =>
        {
          var origins = apiConfiguration.CorsOrigins.Split(',');
          policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
        })
      );
    }

    return apiConfiguration;
  }

  private static void ConfigureApp(WebApplication app, ApiConfiguration apiConfiguration)
  {
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    if (apiConfiguration.AddCors)
    {
      app.UseCors();
    }

    app.UseMiddleware<ApiKeyMiddleware>();
    app.UseAuthorization();

    app.MapControllers();
  }
}
