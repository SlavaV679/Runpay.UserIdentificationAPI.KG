using Azure;
using ELKLogsNS;
using LocalizationHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Runpay.UserIdentification.BisinessLogic.Helpers;
using Runpay.UserIdentification.BisinessLogic.Interfaces;
using Runpay.UserIdentification.DataAccess.Interfaces;
using Runpay.UserIdentification.Domain.Models;
using Runpay.UserIdentification.Domain.Options;
using System.Text;
using System.Text.Json;
using TerminalApiProtocol;

namespace Runpay.UserIdentification.BisinessLogic
{
    public class IdentificationHendler : IIdentificationHendler
    {
        const int SignatureIsNotValidErrorCode = -2;
        const int CertificateNotFoundErrorCode = -3;
        const int CertificateFileIsMissingErrorCode = -4;

        protected Encoding EncodingIn = Encoding.GetEncoding(1251);

        private readonly SignatureOptions _signatureOptions;
        private readonly ILogger<IdentificationHendler> _logger;
        private readonly ITerminalApiClient _terminalApiClient;
        private readonly IPaymentsRepository _paymentsRepository;

        public IdentificationHendler(
            IOptions<SignatureOptions> signatureOptions,
            ILogger<IdentificationHendler> logger,
            ITerminalApiClient terminalApiClient,
        IPaymentsRepository paymentsRepository)

        {
            _signatureOptions = signatureOptions.Value;
            _logger = logger;
            _terminalApiClient = terminalApiClient;
            _paymentsRepository = paymentsRepository;
        }

        public async Task<ResponseType> Handle(string requestRaw, string sign)
        {
            var incomeRequestTest = new IncomeRequest()
            {
                Client = new()
                {
                    Culture = "MD",
                    TerminalId = "N1lin11"
                },
                AuthorizationUser = new()
                {
                    Login = "79046724077",
                    Password = "Hz5g",
                    FromIP = "92.255.199.174"

                },
                Parameters = new()
                {
                    Name = "ParamValue Surname",
                    Surname = "ParamValue Name",
                    Patronymic = "ParamValue Patronymic"
                }
            };

            requestRaw = JsonSerializer.Serialize(incomeRequestTest);

            var incomeRequest = JsonSerializer.Deserialize<IncomeRequest>(requestRaw);

            var requestType = new RequestType()
            {
                Client = new()
                {
                    Culture = incomeRequest.Client.Culture,
                    TerminalId = incomeRequest.Client.TerminalId
                },
                Actions = new()
                {
                    LKP = new()
                    {

                        AuthorizationUser = new()
                        {
                            Login = incomeRequest.AuthorizationUser.Login,
                            Password = incomeRequest.AuthorizationUser.Password,
                            FromIP = incomeRequest.AuthorizationUser.FromIP
                        }
                    }
                }
            };

            //var requestType = RequestType.Deserialize(requestRaw);

            if (_signatureOptions.ValidateSignature)
                await VerifySign("request", "sign", "CertSerial");

            _logger.LogInformation($"Request to TAPI: '{requestType.Serialize()}'");

            var response2 = await _terminalApiClient.AuthorizeUser(requestType);
            
            _logger.LogInformation($"Response from TAPI: '{response2.Serialize()}'");

            if (response2.Data == null)
            {
                _logger.LogInformation($"Authorization error: [{response2.ErrorCode}] '{response2.ErrorText}'");
                throw new TAException(102, "Authorization error", "message for developer");
            }

            var sessionId = response2.Data.LKP.AuthorizationUser.SessionId;

            var request = new RequestType()
            {
                Auth = new()
                {
                    LKSessionId = sessionId
                },
                Client = new()
                {
                    ClientSoftware = requestType.Client.ClientSoftware,
                    Culture = requestType.Client.Culture ?? "MD",
                    TerminalId = requestType.Client.TerminalId
                },
                Actions = new()
                {
                    LKP = new()
                    {
                        SetUserInfo = new List<SetUserParamRequestType>
                        {

                        }
                    }
                }
            };

            var vv1 = new SetUserParamRequestType() { ParamName = "Surname", ParamValue = incomeRequest.Parameters.Surname };
            var vv2 = new SetUserParamRequestType() { ParamName = "Name", ParamValue = incomeRequest.Parameters.Name };
            var vv3 = new SetUserParamRequestType() { ParamName = "Patronymic", ParamValue = incomeRequest.Parameters.Patronymic };

            request.Actions.LKP.SetUserInfo.Add(vv1);
            request.Actions.LKP.SetUserInfo.Add(vv2);
            request.Actions.LKP.SetUserInfo.Add(vv3);

            var response3 = await _terminalApiClient.IdentifyUser(requestType, sessionId);
            if (response3.Data == null)
            {
                _logger.LogInformation($"IdentifyUser error: [{response3.ErrorCode}] '{response3.ErrorText}'");
                throw new TAException(response3?.ErrorCode ?? -5, $"IdentifyUser error {response3.ErrorText}", "message for developer");
            }

            return response3;
        }

        private async Task VerifySign(string originalMessage, string signedMessage, string certSerial)
        {
            throw new TAException(100, "Description error", "message for developer");
        }

        protected async Task VerifySign(string originalMessage, string signedMessage, RequestType requestType)
        {
            var culture = requestType.Client.Culture ?? "MD";

            if (!await _paymentsRepository.CheckCertificate(requestType.CertSerial))
                throw new TAException(
                    CertificateNotFoundErrorCode,
                    LocalizationResourceKey.CertificateNotFound,
                    "Your request has been rejected. Certificate not found or blocked.",
                    culture);

            var publicCertPath = Path.Combine(Path.GetDirectoryName(_signatureOptions.PublicCertPath), requestType.CertSerial + ".cer");

            if (!File.Exists(publicCertPath))
                throw new TAException(
                    CertificateFileIsMissingErrorCode,
                    LocalizationResourceKey.CertificateFileIsMissing,
                    "Certificate file is missing",
                    culture);

            if (!RSAHelper.VerifySignature(originalMessage, EncodingIn, signedMessage, publicCertPath))
            {
                throw new TAException(
                    SignatureIsNotValidErrorCode,
                    LocalizationResourceKey.SignatureIsNotValid,
                    "Sign of request is failed",
                    culture);
            }

            _logger.LogInformation($"Verifying signature is success.");
        }


        public string ProcessException(string requestRaw, Exception ex)
        {
            RequestType request = null;
            try
            {
                request = RequestType.Deserialize(requestRaw);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Request deserialization error: {requestRaw}", exc);
                //throw;
            }
            var _response = new ResponseType();
            if (request != null && request.RequestId != null)
            {
                _response.RequestId = request.RequestId;
            }
            int errCode = -1;
            string errTxt = "Error processing request";
            string errDbg = ex.ToString();
            Exception inEx = ex.InnerException;
            while (inEx != null)
            {
                errDbg += "\n" + inEx.Message;
                inEx = inEx.InnerException;
            }
            if (ex.Data["ERROR_CODE"] != null)
            {
                try
                {
                    errCode = (int)ex.Data["ERROR_CODE"];
                    errTxt = (string)ex.Data["ERROR_USR"];
                    if (ex.Data["ERROR_DBG"] != null)
                        errDbg = (string)ex.Data["ERROR_DBG"];
                }
                catch (Exception ex1)
                {
                    _logger.LogError($"Error installing inner exception information. {ex1}");
                }
            }
            _response.ErrorCode = errCode;
            _response.ErrorText = errTxt;
            _response.DebugText = errDbg;

            if (ex.Data["RESPONSE"] != null)
            {
                _response.Data = (ResponseTypeData)ex.Data["RESPONSE"];
            }

            if (errCode == -1)
            {
                ELKSender.SendEvent(new ELKEvent()
                {
                    Context = new Context
                    {
                        Country = Config.ELK_Country,
                        PosCode = request.Client.TerminalId
                    },
                    Event = new Event()
                    {
                        Level = EnumEventLevel.Error,
                        Result = EnumEventResult.Error,
                        Type = EnumEventType.TA_EXCEPTION,
                    },
                    Exception = new ExceptionInfo()
                    {
                        ErrorCode = errCode,
                        ErrorText = errTxt,
                        DebugText = errDbg,
                        Request = requestRaw
                    }
                });
                _logger.LogError($"{errTxt + Environment.NewLine + errDbg + Environment.NewLine + ex + Environment.NewLine}. The RequestId caused an error: {request?.RequestId}");
            }
            else
            {
                _logger.LogWarning(errTxt + Environment.NewLine + errDbg + Environment.NewLine + ex);
            }

            var responseMsg = _response.Serialize();

            return responseMsg;
        }
    }
}
