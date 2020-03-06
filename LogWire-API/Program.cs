using System;
using LogWire.Controller.Client.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LogWire.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    string endpoint = Environment.GetEnvironmentVariable("lw_controller_endpoint");
                    string token = Environment.GetEnvironmentVariable("lw_access_token");

                    webBuilder.ConfigureAppConfiguration(config =>
                    {
                        config.AddEnvironmentVariables("lw_");
                            config.AddControllerConfiguration(endpoint,"api", token);
                            config.AddControllerConfiguration(endpoint, "services", token);
                    });

                    webBuilder.UseStartup<Startup>();

                });
    }
}
