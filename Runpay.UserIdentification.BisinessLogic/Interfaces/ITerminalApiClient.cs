using TerminalApiProtocol;

namespace Runpay.UserIdentification.BisinessLogic.Interfaces
{
    public interface ITerminalApiClient
    {
        /// <summary>
        /// User identification
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<ResponseType> IdentifyUserDelete(RequestType req);

        Task<ResponseType> AuthorizeUser(RequestType requestType);
        
        Task<ResponseType> IdentifyUser(RequestType requestType, string sessionId);
    }
}
