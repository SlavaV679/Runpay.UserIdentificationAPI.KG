namespace Runpay.UserIdentification.DataAccess.Interfaces
{
    public interface IPaymentsRepository
    {
        Task<bool> CheckCertificate(string serialNumber);
    }
}
