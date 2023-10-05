using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using VAF13.Klubadmin.Domain.Services.Skywin;
using VAF13.Klubadmin.Domain.Services.VAFapi;

namespace VAF13.Klubadmin.UI.Winform
{
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

            services.AddHttpClient<IVAFApiIntegration, VAFApiIntegration>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7287");
                //client.BaseAddress = new Uri("https://echo.free.beeceptor.com");
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.23"));
            });

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(c =>
            {
                c.AddConsole();
                c.SetMinimumLevel(LogLevel.Information);
            });

            services.AddSingleton<Form1>();
            services.AddSingleton<ISkywinMembersDialogService, SkywinMembersDialogService>();
            IServiceProvider sp = services.BuildServiceProvider();

            var form = sp.GetRequiredService<Form1>();
            Application.Run(form);
        }
    }
}