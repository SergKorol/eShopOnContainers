// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using NBomber.CSharp;
using Newtonsoft.Json.Linq;
using Step = NBomber.FSharp.Step;

namespace Coupon.LoadTests
{
    class Program
    {
        static void Main(string[] args)
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
        
        path = $"{path}appsettings.json";
        var json = Json.Net.Curl.Get(path);
        var token = json["Token"].ToString();
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var scenario = Scenario.Create("coupon_scenario", async context =>
            {
                var response = await httpClient.GetAsync($"http://localhost:5106/api/v1/Coupon/DISC-5");
                
        
                return response.IsSuccessStatusCode
                    ? Response.Ok(statusCode: ((int)response.StatusCode).ToString(), message: "Passed")
                    : Response.Fail(statusCode: ((int)response.StatusCode).ToString(), message: "Failed");
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.Inject(rate: 10,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(30))
            );
        
        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        }
    }
}
