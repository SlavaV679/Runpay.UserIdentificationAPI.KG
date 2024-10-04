using NLog.Extensions.Logging;
using NLog;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLogOptions"));

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    loggingBuilder.AddNLog();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/endpoint", () =>
{
    app.Logger.LogError("Check error log from worker.");
    var forecast = "Project started;";
    return forecast;
});

app.Run();

