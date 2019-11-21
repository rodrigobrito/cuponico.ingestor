using System;
using Cuponico.Ingestor.Host.Domain.Advertiser.Stores;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public class AffiliateStoreMatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AdvertiseStoreId { get; set; } = Guid.Empty;
        public long AffiliateStoreId { get; set; }
        public string AffiliateProgram { get; set; }
        public int CouponsCount { get; set; }
        public bool Matched(AffiliateStoreMatch affiliateStore)
        {
            return affiliateStore != null && affiliateStore.AffiliateStoreId == AffiliateStoreId && affiliateStore.AffiliateProgram == AffiliateProgram;
        }

        public void Update(AffiliateStoreMatch storeMatch)
        {
            if (storeMatch == null) throw new ArgumentNullException(nameof(storeMatch));
            if (Matched(storeMatch))
            {
                CouponsCount = storeMatch.CouponsCount;
            }
        }

        public static AffiliateStoreMatch Create(Store store, AffiliateStore affiliateStore)
        {
            var match = affiliateStore.GetAdvertiseId();
            match.AdvertiseStoreId = store.StoreId;
            return match;
        }
    }

    public static class AffiliateStoreMatchExtensions
    {
        public static AffiliateStoreMatch GetAdvertiseId(this AffiliateStore affiliateStore)
        {
            return new AffiliateStoreMatch
            {
                Id = Guid.NewGuid(),
                AffiliateProgram = affiliateStore.AffiliateProgram,
                AffiliateStoreId = affiliateStore.StoreId,
                CouponsCount = affiliateStore.CouponsCount,
            };
        }
    }
}