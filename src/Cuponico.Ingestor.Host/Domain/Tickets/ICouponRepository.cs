using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Tickets
{
    public interface ICouponRepository
    {
        Task<IList<Coupon>> GetAllAsync();
        Task SaveAsync(IList<Coupon> coupons);
        Task DeleteAsync(IList<long> ids);
    }
}