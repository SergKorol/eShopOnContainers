using System.Reflection;
using Coupon.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Coupon.FunctionalTests;

// public class CouponScenarioBase
// {
//     // public TestServer CreateServer()
//     // {
//     //     var path = Assembly.GetAssembly(typeof(CouponScenarioBase))
//     //         .Location;
//     //
//     //     var hostBuilder = new WebHostBuilder()
//     //         .UseContentRoot(Path.GetDirectoryName(path))
//     //         .ConfigureAppConfiguration(cb =>
//     //         {
//     //             cb.AddJsonFile("appsettings.json", optional: false)
//     //             .AddEnvironmentVariables();
//     //         }).UseStartup<OrderingTestsStartup>();
//     //
//     //     var testServer = new TestServer(hostBuilder);
//     //
//     //     testServer.Host
//     //         .MigrateDbContext<CouponContext>((context, services) =>
//     //         {
//     //             var env = services.GetService<IWebHostEnvironment>();
//     //             var settings = services.GetService<IOptions<OrderingSettings>>();
//     //             var logger = services.GetService<ILogger<OrderingContextSeed>>();
//     //
//     //             new OrderingContextSeed()
//     //                 .SeedAsync(context, env, settings, logger)
//     //                 .Wait();
//     //         })
//     //         .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });
//     //
//     //     return testServer;
//     // }
//     //
//     // public static class Get
//     // {
//     //     public static string Orders = "api/v1/orders";
//     //
//     //     public static string OrderBy(int id)
//     //     {
//     //         return $"api/v1/orders/{id}";
//     //     }
//     // }
//     //
//     // public static class Put
//     // {
//     //     public static string CancelOrder = "api/v1/orders/cancel";
//     //     public static string ShipOrder = "api/v1/orders/ship";
//     // }
// }

public class CouponScenarioBase
{
    public TestServer CreateServer()
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

        var hostBuilder = new WebHostBuilder()
            .UseContentRoot(Path.GetDirectoryName(path))
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
            }).UseStartup<CouponTestsStartup>();

        var testServer = new TestServer(hostBuilder);

        return testServer;
    }

    public static class Get
    {
        // public static string Coupons = "api/v1/coupon";

        public static string GetCouponByCode(string code)
        {
            return $"api/v1/coupon/{code}";
        }
    }
}
