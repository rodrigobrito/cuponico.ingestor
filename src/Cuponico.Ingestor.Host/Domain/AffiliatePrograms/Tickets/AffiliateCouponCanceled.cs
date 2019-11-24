using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateCouponCanceled : DomainEvent<long, AffiliateCoupon>
    {
        public AffiliateCouponCanceled()
        {
        }

        protected AffiliateCouponCanceled(long id, AffiliateCoupon @event, DateTime createdDate) : base(id, @event, CuponicoEvents.AffiliateCouponCanceled, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateCoupon>> CreateMany(IList<AffiliateCoupon> coupons)
        {
            var events = new List<DomainEvent<long, AffiliateCoupon>>();
            foreach (var coupon in coupons)
            {
                var storeCreated = Create(coupon.CouponId, coupon, CuponicoEvents.AffiliateCouponCanceled, DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}