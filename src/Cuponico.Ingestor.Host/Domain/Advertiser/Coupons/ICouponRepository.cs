using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Advertiser.Coupons
{
    public interface ICouponRepository
    {
        Task<IList<Coupon>> GetAllAsync();
        Task SaveAsync(Coupon coupon);
        Task SaveAsync(IList<Coupon> coupons);
        Task DeleteAsync(IList<Guid> ids);
    }
}