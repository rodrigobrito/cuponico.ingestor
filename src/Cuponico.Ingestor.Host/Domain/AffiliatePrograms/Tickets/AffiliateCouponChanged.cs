using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateCouponChanged : DomainEvent<long, AffiliateCoupon>
    {
        public AffiliateCouponChanged()
        {
        }

        protected AffiliateCouponChanged(long id, AffiliateCoupon @event, DateTime createdDate) : base(id, @event, CuponicoEvents.AffiliateCouponChanged, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateCoupon>> CreateMany(IList<AffiliateCoupon> coupons)
        {
            var events = new List<DomainEvent<long, AffiliateCoupon>>();
            foreach (var coupon in coupons)
            {
                var storeCreated = Create(coupon.CouponId, coupon, CuponicoEvents.AffiliateCouponChanged, DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}