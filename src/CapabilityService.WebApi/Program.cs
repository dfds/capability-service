using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace DFDS.CapabilityService.WebApi
{
    public class Program
    {
        public static int Main(string[] args)
        {
            bool.TryParse(Environment.GetEnvironmentVariable("CAPABILITY_SERVICE_HUMAN_LOG"), out var humanLog);

            var logcfg = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Internal.WebHost", LogEventLevel.Information)
                .Enrich.FromLogContext();
            
            if (humanLog)
            {
                logcfg.WriteTo.Console(theme: AnsiConsoleTheme.Code);
            }
            else
            {
                logcfg.WriteTo.Console(new CompactJsonFormatter());
            }
            
            Log.Logger = logcfg.CreateLogger();

            try
            {
                Log.Information("Starting host");
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            Console.WriteLine($"process id: {Process.GetCurrentProcess().Id}");

            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var sourcesToRemove = config.Sources
                        .Where(s => s.GetType() == typeof(JsonConfigurationSource))
                        .ToArray();
                    
                    foreach (var source in sourcesToRemove)
                    {
                        config.Sources.Remove(source);
                    }
                    
                    config
                        .AddJsonFile(
                            path: "appsettings.json",
                            optional: true,
                            reloadOnChange: false
                        )
                        .AddJsonFile(
                            path: "appsettings." + builderContext.HostingEnvironment.EnvironmentName + ".json",
                            optional: true,
                            reloadOnChange: false
                        );
                })
                .UseSerilog()
                .UseStartup<Startup>();
        }
    }
}