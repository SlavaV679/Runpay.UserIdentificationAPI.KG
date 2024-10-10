using TerminalApiProtocol;

namespace Runpay.UserIdentification.BisinessLogic.Interfaces
{
    public interface IIdentificationHendler
    {
        Task<ResponseType> Handle(string requestRaw, string sign);

        string ProcessException(string requestRaw, Exception ex);
    }
}
