using Runpay.UserIdentification.DataAccess.IdentifyDoc;

namespace Runpay.UserIdentification.DataAccess.Interfaces
{
    public interface IIdentifyDocRepository
    {
        Task<int> AddRequest(Request request);
        
        Task<Request?> GetRequestByPhone(string phone);
    }
}
