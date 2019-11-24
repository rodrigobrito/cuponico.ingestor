using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateCouponCreated : DomainEvent<long, AffiliateCoupon>
    {
        public AffiliateCouponCreated()
        {
        }

        protected AffiliateCouponCreated(long id, AffiliateCoupon @event, DateTime createdDate) : base(id, @event, CuponicoEvents.AffiliateCouponCreated, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateCoupon>> CreateMany(IList<AffiliateCoupon> coupons)
        {
            var events = new List<DomainEvent<long, AffiliateCoupon>>();
            foreach (var coupon in coupons)
            {
                var storeCreated = Create(coupon.CouponId, coupon, CuponicoEvents.AffiliateCouponCreated, DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}