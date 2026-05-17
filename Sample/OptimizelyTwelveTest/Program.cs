namespace OptimizelyTwelveTest;

using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == Environments.Development)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureCmsDefaults()
                       .ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigureKestrel((context, options) =>
                       {
                           // Add valid HTTPS ports for development that are compatible with Opti Id
                           options.ListenLocalhost(5000, options => { options.UseHttps(); });
                           options.ListenLocalhost(5001, options => { options.UseHttps(); });
                           options.ListenLocalhost(5002, options => { options.UseHttps(); });
                           options.ListenLocalhost(5003, options => { options.UseHttps(); });

                           options.Limits.MaxRequestBodySize = 2147483648;
                           options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(60);
                       })
                       .UseStartup<Startup>());
        }
        else
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureCmsDefaults()
                       .ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigureKestrel((context, options) =>
                       {
                           options.Limits.MaxRequestBodySize = 2147483648;
                           options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(60);
                       })
                       .UseStartup<Startup>());
        }
    }
}