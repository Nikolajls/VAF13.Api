using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http.Headers;
using VAF13.Klubadmin.Domain.Configurations;
using VAF13.Klubadmin.Domain.Services.Skywin;
using VAF13.Klubadmin.Domain.Services.VAFapi;

namespace VAF13.Klubadmin.UI.Winform;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        IServiceCollection services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables();

        var configuration = configurationBuilder.Build();
        services.AddOptions();
        services.Configure<UiAppConfiguration>(configuration.GetSection(UiAppConfiguration.ConfigurationSectionName));
            
        //
        services.AddHttpClient<IVafApiIntegration, VafApiIntegration>((sp, client) =>
        {
            var logger = sp.GetRequiredService<ILogger<VAFApiIntegrationHttpClient>>();
            var uiAppConfiguration = sp.GetRequiredService<IOptions<UiAppConfiguration>>().Value;

            var apiEndpoint = uiAppConfiguration.ApiEndpoint ?? string.Empty;
            var apiKey = uiAppConfiguration.APIKey ?? string.Empty;
            logger.LogInformation("HTTP endpoint: {Http} Key: {ApiKey}" , apiEndpoint, apiKey.Substring(0,1));

            client.BaseAddress = new Uri(apiEndpoint);
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.23"));
        });

        // Logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel
            .Verbose()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}", $"{DateTime.Now.Month}-{DateTime.Now.Day}-Log.txt"),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(x =>
        {
            x.ClearProviders();
            x.AddSerilog(dispose: true);
        });

        //
        services.AddSingleton<IWindowsApiService, WindowsApiService>();
        services.AddSingleton<Form1>();
        services.AddSingleton<ISkywinMembersDialogService, SkywinMembersDialogService>();
        IServiceProvider sp = services.BuildServiceProvider();

        var form = sp.GetRequiredService<Form1>();
        Application.Run(form);
    }
}
