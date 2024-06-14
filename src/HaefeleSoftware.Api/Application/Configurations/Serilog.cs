using Microsoft.Extensions.Logging;
using Serilog;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Serilog
{
    public static void AddSerilog(this IServiceCollection service)
    {
        var serilogFile = new ConfigurationBuilder()
            .AddJsonFile("serilog.setup.json")
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(serilogFile)
            .CreateLogger();

        service.AddSingleton(Log.Logger);
        service.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });
    }
}