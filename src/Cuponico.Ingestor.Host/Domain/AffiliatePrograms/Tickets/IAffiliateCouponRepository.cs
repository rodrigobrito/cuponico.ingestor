using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public interface IAffiliateCouponRepository
    {
        Task<IList<AffiliateCoupon>> GetAllAsync();
        Task SaveAsync(IList<AffiliateCoupon> coupons);
        Task DeleteAsync(IList<long> ids);
    }
}