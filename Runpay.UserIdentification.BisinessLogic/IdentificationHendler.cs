using ELKLogsNS;
using LocalizationHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Payments.GateWay.GateWayCommon;
using Runpay.UserIdentification.BisinessLogic.Helpers;
using Runpay.UserIdentification.BisinessLogic.Interfaces;
using Runpay.UserIdentification.DataAccess.IdentifyDoc;
using Runpay.UserIdentification.DataAccess.Interfaces;
using Runpay.UserIdentification.Domain.Enums;
using Runpay.UserIdentification.Domain.Extensions;
using Runpay.UserIdentification.Domain.Models;
using Runpay.UserIdentification.Domain.Options;
using System.Globalization;
using System;
using System.Security.Cryptography;
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

        //protected Encoding EncodingIn = Encoding.GetEncoding(1251);

        private readonly SignatureOptions _signatureOptions;
        private readonly ApiOptions _apiOptions;
        private readonly ILogger<IdentificationHendler> _logger;
        private readonly ITerminalApiClient _terminalApiClient;
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly IIdentifyDocRepository _identifyDocRepository;

        public IdentificationHendler(
            IOptions<SignatureOptions> signatureOptions,
            IOptions<ApiOptions> apiOptions,
            ILogger<IdentificationHendler> logger,
            ITerminalApiClient terminalApiClient,
        IPaymentsRepository paymentsRepository,
        IIdentifyDocRepository identifyDocRepository)

        {
            _signatureOptions = signatureOptions.Value;
            _apiOptions = apiOptions.Value;
            _logger = logger;
            _terminalApiClient = terminalApiClient;
            _paymentsRepository = paymentsRepository;
            _identifyDocRepository = identifyDocRepository;
        }

        public async Task<ResponseToClientRaw> Handle(string requestRaw)
        {
            var incomeRequestRow = IncomeRequestRaw.Deserialize(requestRaw);

            if (!DateTime.TryParseExact(incomeRequestRow.PersonalData.DateOfBirth, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                throw new IdentifyException(ErrorCodeEnum.IncorrectDateOfBirth);

            if (string.IsNullOrWhiteSpace(incomeRequestRow.Signwmid)
                || string.IsNullOrWhiteSpace(incomeRequestRow.Sign)
                || string.IsNullOrWhiteSpace(incomeRequestRow.PersonalData.Phone)
                || string.IsNullOrWhiteSpace(incomeRequestRow.PersonalData.FirstName)
                || string.IsNullOrWhiteSpace(incomeRequestRow.PersonalData.LastName)
                || string.IsNullOrWhiteSpace(incomeRequestRow.PersonalData.MiddleName)
                || string.IsNullOrWhiteSpace(incomeRequestRow.PersonalData.IssuingCountry)
                || string.IsNullOrWhiteSpace(incomeRequestRow.PersonalData.PassportNumber)
                )
                throw new IdentifyException(ErrorCodeEnum.IncorrectRequestFormat);

            Request requestDb = new Request()
            {
                PosCode = _apiOptions.PosCode,
                Phone = incomeRequestRow.PersonalData.Phone,
                DocType = "Passport",
                DateAdd = DateTime.Now,
                IsRegistered = false,
                RegistrationTypeId = (int)RegistrationType.SimplisticallyIdentifiedNaturalPerson,
                BillTypeId = (int)AccountType.UPESP,
                IsConfirmed = false
            };

            requestDb.User = new User()
            {
                Idnp = incomeRequestRow.PersonalData.PassportNumber,
                Name = incomeRequestRow.PersonalData.FirstName,
                LastName = incomeRequestRow.PersonalData.LastName,
                MiddleName = incomeRequestRow.PersonalData.MiddleName,
                TypeId = (int)EnumUserTypes.TelepayRu
            };

            requestDb.User.Document = new Document
            {
                BirthDate = birthDate,
                Beneficiar = $"{incomeRequestRow.PersonalData.FirstName} {incomeRequestRow.PersonalData.LastName}",
                Country = incomeRequestRow.PersonalData.IssuingCountry,
                AddressResidence = null,//$"{respPerson.Registration.Address.Region}, {respPerson.Registration.Address.Locality}, {respPerson.Registration.Address.Street} {respPerson.Registration.Address.House}, {respPerson.Registration.Address.Flat}",

                TypeId = 1,
                SerialNumber = incomeRequestRow.PersonalData.PassportNumber
            };


            if (_signatureOptions.ValidateSignature)
                await VerifySign(incomeRequestRow);

            var requestFromDb = await _identifyDocRepository.GetRequestByPhone(incomeRequestRow.PersonalData.Phone);
            if (requestFromDb != null)
                throw new IdentifyException(ErrorCodeEnum.ProfileAlreadyExists);

            var requestId = await _identifyDocRepository.AddRequest(requestDb);
            if (requestId == 0)
                throw new IdentifyException(ErrorCodeEnum.InternalServerError);

            return new ResponseToClientRaw()
            {
                ReturnedValue = ((int)ErrorCodeEnum.SuccessResultCode).ToString(),
                ReturnedDescription = ErrorCodeEnum.SuccessResultCode.GetDescriptionByEnumValue()
            };

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

            return null;
        }

        private async Task VerifySign(IncomeRequestRaw incomeRequestRow)
        {
            var stringForSign = $"{incomeRequestRow.Signwmid};{incomeRequestRow.PersonalData.PassportNumber};{incomeRequestRow.PersonalData.IssuingCountry};{_signatureOptions.SecretKey}";

            var signAsSHA256Hash = GetSHA256Hash(stringForSign, Encoding.UTF8);

            if (signAsSHA256Hash != incomeRequestRow.Sign)
                throw new IdentifyException(ErrorCodeEnum.SignatureIsNotValid);
        }

        public static string GetSHA256Hash(string input, Encoding encoding)
        {
            byte[] array = SHA256.Create().ComputeHash(encoding.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public string ProcessException(string requestRaw, Exception ex)
        {
            IncomeRequestRaw request = null;
            try
            {
                request = IncomeRequestRaw.Deserialize(requestRaw);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Request deserialization error: {requestRaw}", exc);
            }

            var phone = string.Empty;
            if (request != null && request.PersonalData.Phone != null)
            {
                phone = request.PersonalData.Phone;
            }

            string errTxt = $"Error processing request {phone}";
            string errDbg = ex.ToString();

            Exception inEx = ex.InnerException;
            while (inEx != null)
            {
                errDbg += "\n" + inEx.Message;
                inEx = inEx.InnerException;
            }

            _logger.LogWarning(errTxt + Environment.NewLine + errDbg + Environment.NewLine + ex);

            var response = new ResponseToClientRaw()
            {
                ReturnedValue = ((int)ErrorCodeEnum.ServerError).ToString(),
                ReturnedDescription = ErrorCodeEnum.ServerError.GetDescriptionByEnumValue()
            };

            var responseMsg = ResponseToClientRaw.Serialize(response);

            return responseMsg;
        }
    }
}
