
using System.Net.Http.Headers;
using VAF13.Klubadmin.Domain.Configurations;
using VAF13.Klubadmin.Domain.Infrastructure;
using VAF13.Klubadmin.Domain.Services.Klubadmin;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            ConfigureServices(services, builder.Configuration);

            var app = builder.Build();
            ConfigureApp(app);
            app.Run();
        }


        private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddOptions();
            services.Configure<DFUConfiguration>(configuration.GetSection("DfuConfiguration"));

            //
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

            //
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

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
        }

    }
}