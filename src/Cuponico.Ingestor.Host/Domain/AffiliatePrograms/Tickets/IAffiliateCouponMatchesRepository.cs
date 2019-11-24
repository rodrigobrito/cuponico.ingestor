using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public interface IAffiliateCouponMatchesRepository
    {
        Task<IList<AffiliateCouponMatch>> GetAllAsync();
        Task SaveAsync(AffiliateCouponMatch coupon);
        Task DeleteAsync(IList<Guid> ids);
    }
}
