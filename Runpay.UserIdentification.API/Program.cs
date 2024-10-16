using NLog.Extensions.Logging;
using NLog;
using Runpay.UserIdentification.BisinessLogic.Interfaces;
using TerminalApiProtocol;
using Runpay.UserIdentification.Domain.Options;
using Runpay.UserIdentification.BisinessLogic;
using Runpay.UserIdentification.BisinessLogic.Helpers;
using Runpay.UserIdentification.Domain.Models;
using System.Net.Mime;
using Runpay.UserIdentification.DataAccess.Interfaces;
using Runpay.UserIdentification.DataAccess.Repository;
using Runpay.UserIdentification.DataAccess.Payments;
using Runpay.UserIdentification.DataAccess.IdentifyDoc;

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

builder.Services.AddOptions<TerminalApiOptions>().BindConfiguration(nameof(TerminalApiOptions)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ApiOptions>().BindConfiguration(nameof(ApiOptions)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<SignatureOptions>().BindConfiguration(nameof(SignatureOptions)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ELKOptions>().BindConfiguration(nameof(ELKOptions)).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddScoped<PaymentsContext>();
builder.Services.AddScoped<IdentifyDocContext>();

builder.Services.AddTransient<IIdentificationHendler, IdentificationHendler>();
builder.Services.AddTransient<ITerminalApiClient, TerminalApiClient>();
builder.Services.AddTransient<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddTransient<IIdentifyDocRepository, IdentifyDocRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/identification/api", async (HttpRequest httpRequest, IIdentificationHendler identificationHendler) =>
{
    string requestRaw = string.Empty;
    string responseString = string.Empty;

    try
    {
        using var reader = new StreamReader(httpRequest.Body);
        requestRaw = await reader.ReadToEndAsync();

        app.Logger.LogInformation($"Request: {requestRaw}");

        var response = await identificationHendler.Handle(requestRaw);

        responseString = ResponseToClientRaw.Serialize(response);
        app.Logger.LogInformation($"Response: {responseString}");
    }
    catch (IdentifyException ex)
    {
        var response = new ResponseToClientRaw()
        {
            ReturnedValue = ex.ErrorCode.ToString(),
            ReturnedDescription = ex.ErrorUserMessage
        };

        responseString = ResponseToClientRaw.Serialize(response);
    }
    catch (Exception ex)
    {
        responseString = identificationHendler.ProcessException(requestRaw, ex);
    }

    return Results.Text(responseString, MediaTypeNames.Text.Xml);
});

app.Run();

