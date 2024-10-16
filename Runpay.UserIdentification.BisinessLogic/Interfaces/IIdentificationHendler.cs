using Runpay.UserIdentification.Domain.Models;
using TerminalApiProtocol;

namespace Runpay.UserIdentification.BisinessLogic.Interfaces
{
    public interface IIdentificationHendler
    {
        Task<ResponseToClientRaw> Handle(string requestRaw);

        string ProcessException(string requestRaw, Exception ex);
    }
}
