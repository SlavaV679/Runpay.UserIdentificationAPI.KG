using System.Reflection.Metadata.Ecma335;

namespace Runpay.UserIdentification.Domain.Models
{
    public class IncomeRequest
    {
        public string Version { get; set; }
        public string RequestId { get; set; }
        public Auth Auth { get; set; }
        public AuthorizationUser AuthorizationUser { get; set; }
        public Client Client { get; set; }
        public Actions Actions { get; set; }
        public Parameters Parameters { get; set; }
    }

    public class Parameters
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
    }

    public class AuthorizationUser
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string FromIP { get; set; }
    }

    public class Auth
    {
        public string LKSessionId { get; set; }
    }

    public class Client
    {
        public string ClientSoftware { get; set; }
        public string Culture { get; set; }
        public string TerminalId { get; set; }
    }

    public class Actions
    {
        public LKP LKP { get; set; }
    }

    public class LKP
    {
        public List<SetUserInfo> SetUserInfo { get; set; }
    }

    public class SetUserInfo
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
    }
}
