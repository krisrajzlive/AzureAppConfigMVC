using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using System;
using Azure.Identity;

namespace AzureAppConfigMVC
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
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var azAppConfigSettings = config.Build();
                        var azAppConfigConnection = azAppConfigSettings["AppConfig"];
                        config.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(azAppConfigConnection)
                                   .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
                                   .ConfigureRefresh(refresh =>
                                   {
                                       refresh.Register("TestApp:Settings:Sentinel", refreshAll: true)
                                              .SetCacheExpiration(new TimeSpan(0, 0, 1));
                                   })
                                   .ConfigureRefresh(refresh =>
                                   {
                                       refresh.Register("TestApp:Settings:Message", hostingContext.HostingEnvironment.EnvironmentName)
                                              .SetCacheExpiration(TimeSpan.FromDays(1));
                                   });
                        });
                    })
                .UseStartup<Startup>());

    }
}
