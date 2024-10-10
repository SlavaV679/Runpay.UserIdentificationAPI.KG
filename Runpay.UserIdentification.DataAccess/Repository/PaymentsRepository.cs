using Microsoft.EntityFrameworkCore;
using Runpay.UserIdentification.DataAccess.Interfaces;
using Runpay.UserIdentification.DataAccess.Payments;

namespace Runpay.UserIdentification.DataAccess.Repository
{
    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly PaymentsContext _context;

        public PaymentsRepository(PaymentsContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckCertificate(string serialNumber)
        {
            var certificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.SerialNumber == serialNumber);

            if (certificate == null) { return false; }

            return certificate.State ?? false;
        }
    }
}
