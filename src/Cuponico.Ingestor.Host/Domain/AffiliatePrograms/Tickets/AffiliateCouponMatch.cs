using System;
using Cuponico.Ingestor.Host.Domain.Advertiser.Coupons;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateCouponMatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AdvertiseCouponId { get; set; } = Guid.Empty;
        public long AffiliateCouponId { get; set; }
        public string AffiliateProgram { get; set; }

        public bool Matched(AffiliateCouponMatch affiliateStore)
        {
            return affiliateStore != null && affiliateStore.AffiliateCouponId == AffiliateCouponId && affiliateStore.AffiliateProgram == AffiliateProgram;
        }

        public static AffiliateCouponMatch Create(Coupon coupon, AffiliateCoupon affiliateCoupon)
        {
            var match = affiliateCoupon.GetAdvertiseId();
            match.AdvertiseCouponId = coupon.CouponId;
            return match;
        }
    }

    public static class AffiliateCouponMatchExtensions
    {
        public static AffiliateCouponMatch GetAdvertiseId(this AffiliateCoupon affiliateCoupon)
        {
            return new AffiliateCouponMatch
            {
                Id = Guid.NewGuid(),
                AffiliateCouponId = affiliateCoupon.CouponId,
                AffiliateProgram = affiliateCoupon.AffiliateProgram
            };
        }
    }
}