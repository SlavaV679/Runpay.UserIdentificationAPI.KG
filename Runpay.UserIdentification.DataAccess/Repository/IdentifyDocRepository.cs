using Microsoft.EntityFrameworkCore;
using Runpay.UserIdentification.DataAccess.IdentifyDoc;
using Runpay.UserIdentification.DataAccess.Interfaces;

namespace Runpay.UserIdentification.DataAccess.Repository
{
    public class IdentifyDocRepository : IIdentifyDocRepository
    {
        private readonly IdentifyDocContext _context;
        public IdentifyDocRepository(IdentifyDocContext context)
        {
            _context = context;
        }

        public async Task<int> AddRequest(Request request)
        {
            var requestEntity = await _context.Requests.AddAsync(request);

            await _context.SaveChangesAsync();

            return requestEntity.Entity.Id;
            //return request.Id; проверить что это одно и то же.
        }

        public async Task<Request?> GetRequestByPhone(string phone)
        {
            return await _context.Requests.FirstOrDefaultAsync(r => r.Phone == phone);
        }
    }
}
