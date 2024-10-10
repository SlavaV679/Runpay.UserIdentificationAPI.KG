using NLog.Extensions.Logging;
using NLog;
using Runpay.UserIdentification.BisinessLogic.Interfaces;
using TerminalApiProtocol;
using Runpay.UserIdentification.Domain.Options;
using Runpay.UserIdentification.BisinessLogic;
using Runpay.UserIdentification.BisinessLogic.Helpers;
using Runpay.UserIdentification.Domain.Models;
using System.Net.Mime;

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
builder.Services.AddOptions<SignatureOptions>().BindConfiguration(nameof(SignatureOptions)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ELKOptions>().BindConfiguration(nameof(ELKOptions)).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddScoped<ITerminalApiClient, TerminalApiClient>();
builder.Services.AddScoped<IIdentificationHendler, IdentificationHendler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/identification/api", async (HttpRequest httpRequest, IIdentificationHendler identificationHendler) =>
{
    ResponseType response = null;
    string requestRaw = string.Empty;
    string responseRaw = string.Empty;

    try
    {
        //TODO check  sign in header
        //TODO check  request body

        //var certificateSerialNumber = httpRequest.HttpContext.Connection?.ClientCertificate?.SerialNumber.Replace("-", "");
        //var certSubject = CertificateMisc.NameToCN(httpRequest.HttpContext.Connection?.ClientCertificate?.Subject ?? string.Empty);

        //app.Logger.LogInformation($"Начало обработки запроса. Данные клиентского сертификата: Serial=[{certificateSerialNumber}],Subject=[{certSubject}]");

        using var reader = new StreamReader(httpRequest.Body);
        requestRaw = await reader.ReadToEndAsync();

        app.Logger.LogInformation($"Request: {requestRaw}");

        var sign = httpRequest.Headers["Sign"].ToString();
        if (string.IsNullOrWhiteSpace(sign))
        {
            //throw new TAException(101, "description sign", "");
        }
        response = new ResponseType();
        response = await identificationHendler.Handle(requestRaw, sign);//, certificateSerialNumber);

        responseRaw = response.Serialize();
        app.Logger.LogInformation($"Response: {responseRaw}");
    }
    catch (Exception ex)
    {
        responseRaw = identificationHendler.ProcessException(requestRaw, ex);
    }

    return Results.Text(responseRaw, MediaTypeNames.Text.Xml);
});

app.Run();

