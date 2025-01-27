using Autofac.Extensions.DependencyInjection;
using Coupon.API.Extensions;
using Coupon.API.Infrastructure;
using Coupon.API.Infrastructure.Repositories;
using Coupon.API.Infrastructure.Repositories.Point;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Coupon.API
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .SeedDatabaseStrategy<CouponContext>(context => new CouponSeed().SeedAsync(context).Wait())
                .SeedDatabaseStrategy<PointContext>(context => new PointSeed().SeedAsync(context).Wait())
                .SubscribersIntegrationEvents()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((host, builder) =>
                {
                    var path = Directory.GetCurrentDirectory();
                    Console.WriteLine("PATH SETTINGS:");
                    Console.WriteLine(path);
                    if (path.EndsWith("bin/Debug/net6.0"))
                    {
                        path = path.Remove(path.Length-16);
                    }

                    if (path.EndsWith("/bin/Release/net6.0"))
                    {
                        path = path.Remove(path.Length-18);
                    }

                    builder.SetBasePath(path)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    var config = builder.Build();

                    if (config.GetValue("UseVault", false))
                    {
                        builder.AddAzureKeyVault($"https://{config["Vault:Name"]}.vault.azure.net/", config["Vault:ClientId"], config["Vault:ClientSecret"]);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseSerilog((host, builder) =>
                {
                    builder.MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.WithProperty("ApplicationContext", host.HostingEnvironment.ApplicationName)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Seq((string.IsNullOrWhiteSpace(host.Configuration["Serilog:SeqServerUrl"]) ? "http://seq" : host.Configuration["Serilog:SeqServerUrl"]) ?? string.Empty)
                        .WriteTo.Http((string.IsNullOrWhiteSpace(host.Configuration["Serilog:LogstashgUrl"]) ? "http://logstash:8080" : host.Configuration["Serilog:LogstashgUrl"]) ?? string.Empty)
                        .ReadFrom.Configuration(host.Configuration);
                });
    }
}
