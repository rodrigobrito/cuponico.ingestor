using System;
using Cuponico.Ingestor.Host.Domain.Advertiser.Categories;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategoryMatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AdvertiseCategoryId { get; set; } = Guid.Empty;
        public long AffiliateCategoryId { get; set; }
        public string AffiliateProgram { get; set; }
        public int CouponsCount { get; set; }
        public bool Matched(AffiliateCategoryMatch affiliateCategory)
        {
            return affiliateCategory != null && affiliateCategory.AffiliateCategoryId == AffiliateCategoryId && affiliateCategory.AffiliateProgram == AffiliateProgram;
        }

        public void Update(AffiliateCategoryMatch categoryMatch)
        {
            if (categoryMatch == null) throw new ArgumentNullException(nameof(categoryMatch));
            if (Matched(categoryMatch))
            {
                CouponsCount = categoryMatch.CouponsCount;
            }
        }

        public static AffiliateCategoryMatch Create(Category category, AffiliateCategory affiliateCategory)
        {
            var match = affiliateCategory.GetAdvertiseId();
            match.AdvertiseCategoryId = category.CategoryId;
            return match;
        }
    }

    public static class AffiliateCategoryMatchExtensions
    {
        public static AffiliateCategoryMatch GetAdvertiseId(this AffiliateCategory category)
        {
            return new AffiliateCategoryMatch
            {
                Id = Guid.NewGuid(),
                AffiliateProgram = category.AffiliateProgram,
                AffiliateCategoryId = category.CategoryId,
                CouponsCount = category.CouponsCount,
            };
        }
    }
}
