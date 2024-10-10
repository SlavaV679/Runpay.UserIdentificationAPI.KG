using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Runpay.UserIdentification.BisinessLogic.Interfaces;
using Runpay.UserIdentification.Domain.Options;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using TerminalApiProtocol;

namespace Runpay.UserIdentification.BisinessLogic.Helpers
{
    public class TerminalApiClient : ITerminalApiClient
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOptions<TerminalApiOptions> _terminalApiOptions;

        public TerminalApiClient(IHostingEnvironment hostingEnvironment, IOptions<TerminalApiOptions> terminalApiOptions)
        {
            _hostingEnvironment = hostingEnvironment;
            _terminalApiOptions = terminalApiOptions;
        }

        private async Task<TResponse> GetXmlResponse<TRequest, TResponse>(TRequest req)
        {
            var certificatePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/N1T309.p12");
            if (certificatePath == null || !System.IO.File.Exists(certificatePath))
            {
                throw new Exception($"Certificate file not found: {certificatePath}");
            }
            var certificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath),
                "595",
                    X509KeyStorageFlags.MachineKeySet
                            | X509KeyStorageFlags.PersistKeySet
                            | X509KeyStorageFlags.Exportable);


            var messageHandler = new HttpClientHandler();


            messageHandler.ClientCertificates.Add(certificate);
            messageHandler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslPolicyErrors) => true;


            var url = _terminalApiOptions.Value.Url;
            var httpClient = HttpClientFactory.Create(messageHandler, null);
            httpClient.BaseAddress = new Uri(url);
            var serializer = new XmlSerializer(typeof(TRequest));
            var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, req);

            memoryStream.Position = 0;
            var streamContent = new StreamContent(memoryStream);
            var test = await streamContent.ReadAsStringAsync();
            var res1 = await httpClient.GetAsync(url);
            var res = await httpClient.PostAsync(url, streamContent);
            var strRes = await res.Content.ReadAsStringAsync();
            var stream = await res.Content.ReadAsStreamAsync();
            var deserializer = new XmlSerializer(typeof(TResponse));
            var response = (TResponse)deserializer.Deserialize(stream);
            return response;
        }

        public async Task<ResponseType> IdentifyUser3(RequestType requestType)
        {
            //var newRequest = requestType.Actions.IdentifyDoc.NewRequest;

            var request = new RequestType
            {
                Client = new ClientRequestType()
                {
                    ClientSoftware = requestType.Client.ClientSoftware,
                    Culture = requestType.Client.Culture ?? "MD",
                    TerminalId = requestType.Client.TerminalId
                },
                Actions = new()
                {
                    IdentifyDoc = new()
                    {
                        NewRequest = new()
                        {
                            //Phone = newRequest.Phone,
                            //DocType = newRequest.DocType,
                            //IDNP = newRequest.IDNP,
                            //CheckPhone = newRequest.CheckPhone,
                            //CheckPhoneSpecified = newRequest.CheckPhoneSpecified,
                            //CreatePhone = newRequest.CreatePhone,
                            //CreatePhoneSpecified = newRequest.CreatePhoneSpecified,
                            //SendOTP = newRequest.SendOTP,
                            //SendOTPSpecified = newRequest.SendOTPSpecified,
                            //Birthdate = newRequest.Birthdate,
                            //BirthdateSpecified = newRequest.BirthdateSpecified
                        }
                    }
                }
            };

            //if (newRequest.AccountType == RequestTypeActionsIdentifyDocNewRequestAccountType.UPESP)
            //    request.Actions.IdentifyDoc.NewRequest.AccountType = newRequest.AccountType;

            var response = await GetXmlResponse<RequestType, ResponseType>(request);
            return response;
        }
        public async Task<ResponseType> IdentifyUserDelete(RequestType requestType)
        {
            //var newRequest = requestType.Actions.IdentifyDoc.NewRequest;

            var request = new RequestType
            {
                Client = new ClientRequestType()
                {
                    ClientSoftware = requestType.Client.ClientSoftware,
                    Culture = requestType.Client.Culture ?? "MD",
                    TerminalId = "N1lin11"//requestType.Client.TerminalId
                },
                Actions = new()
                {
                    LKP = new()
                    { GetEnterpriseInfo = new() }
                    //IdentifyDoc = new()
                    //{
                    //    NewRequest = new()
                    //    {
                    //        Phone = newRequest.Phone,
                    //        DocType = newRequest.DocType,
                    //        IDNP = newRequest.IDNP,
                    //        CheckPhone = newRequest.CheckPhone,
                    //        CheckPhoneSpecified = newRequest.CheckPhoneSpecified,
                    //        CreatePhone = newRequest.CreatePhone,
                    //        CreatePhoneSpecified = newRequest.CreatePhoneSpecified,
                    //        SendOTP = newRequest.SendOTP,
                    //        SendOTPSpecified = newRequest.SendOTPSpecified,
                    //        Birthdate = newRequest.Birthdate,
                    //        BirthdateSpecified = newRequest.BirthdateSpecified
                    //    }
                    //}
                }
            };

            //if (newRequest.AccountType == RequestTypeActionsIdentifyDocNewRequestAccountType.UPESP)
            //    request.Actions.IdentifyDoc.NewRequest.AccountType = newRequest.AccountType;

            var response = await GetXmlResponse<RequestType, ResponseType>(request);
            return response;
        }


        //todo GetResponse
        public async Task<ResponseType> AuthorizeUser(RequestType requestType)
        {            
            var response = new ResponseType();

            try
            {
                response = await GetXmlResponse<RequestType, ResponseType>(requestType);
            }
            catch (Exception ex)
            {
                response.ErrorText = ex.Message;
            }
            
            return response;
        }

        public async Task<ResponseType> IdentifyUser(RequestType requestType, string sessionId)
        {
            //var request = new RequestType()
            //{
            //    Auth = new()
            //    {
            //        LKSessionId = sessionId
            //    },
            //    Client = new()
            //    {
            //        ClientSoftware = requestType.Client.ClientSoftware,
            //        Culture = requestType.Client.Culture ?? "MD",
            //        TerminalId = requestType.Client.TerminalId
            //    },
            //    Actions = new()
            //    {
            //        LKP = new()
            //        {
            //            SetUserInfo = new List<SetUserParamRequestType>
            //            {

            //            }
            //        }
            //    }
            //};
            //var vv1 = new SetUserParamRequestType() { ParamName = "Surname", ParamValue = "ParamValue Surname" };
            //var vv2 = new SetUserParamRequestType() { ParamName = "Name", ParamValue = "ParamValue Name" };
            //var vv3 = new SetUserParamRequestType() { ParamName = "Patronymic", ParamValue = "ParamValue Patronymic" };

            //request.Actions.LKP.SetUserInfo.Add(vv1);
            //request.Actions.LKP.SetUserInfo.Add(vv2);
            //request.Actions.LKP.SetUserInfo.Add(vv3);

            var response = await GetXmlResponse<RequestType, ResponseType>(requestType);
            return response;
        }
    }
}

