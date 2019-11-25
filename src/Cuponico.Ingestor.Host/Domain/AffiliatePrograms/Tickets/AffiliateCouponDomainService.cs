using Cuponico.Ingestor.Host.Domain.Advertiser.Coupons;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets
{
    public class AffiliateCouponDomainService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IAffiliateCouponMatchesRepository _matchesRepository;
        private readonly IAffiliateStoreMatchesRepository _storeRepository;
        private readonly IAffiliateCategoryMatchesRepository _categoryRepository;
        public AffiliateCouponDomainService(ICouponRepository repository,
            IAffiliateCouponMatchesRepository matchesRepository,
            IAffiliateStoreMatchesRepository storeRepository,
            IAffiliateCategoryMatchesRepository categoryRepository)
        {
            _couponRepository = repository.ThrowIfNull(nameof(repository));
            _matchesRepository = matchesRepository.ThrowIfNull(nameof(matchesRepository));
            _storeRepository = storeRepository.ThrowIfNull(nameof(storeRepository));
            _categoryRepository = categoryRepository.ThrowIfNull(nameof(categoryRepository));
        }

        public async Task ProcessCoupon(AffiliateCoupon affiliateCoupon)
        {
            if (affiliateCoupon == null)
                throw new ArgumentNullException(nameof(affiliateCoupon));

            var couponsMatches = await _matchesRepository.GetAllAsync();
            var categoriesMatches = await _categoryRepository.GetAllAsync();
            var storesMatches = await _storeRepository.GetAllAsync();

            var matchedCoupon = couponsMatches.FirstOrDefault(matched =>
                matched.AffiliateProgram == affiliateCoupon.AffiliateProgram &&
                matched.AffiliateCouponId == affiliateCoupon.CouponId);

            if (matchedCoupon != null)
            {
                var coupons = await _couponRepository.GetAllAsync();
                var couponToChange = coupons.FirstOrDefault(c => c.CouponId == matchedCoupon.AdvertiseCouponId); 
                UpdateProperties(couponToChange, affiliateCoupon, storesMatches, categoriesMatches);
                await _couponRepository.SaveAsync(couponToChange);
                return;
            }

            var newCoupon = Coupon.Create();
            UpdateProperties(newCoupon, affiliateCoupon, storesMatches, categoriesMatches);
            matchedCoupon = AffiliateCouponMatch.Create(newCoupon, affiliateCoupon);
            await _matchesRepository.SaveAsync(matchedCoupon);
            await _couponRepository.SaveAsync(newCoupon);
        }

        private static void UpdateProperties(Coupon coupon,
            AffiliateCoupon affiliateCoupon,
            IList<AffiliateStoreMatch> storesMatches,
            IList<AffiliateCategoryMatch> categoriesMatches)
        {
            coupon.ChangedDate = DateTime.UtcNow;
            coupon.Code = affiliateCoupon.Code;
            coupon.CouponLink = affiliateCoupon.CouponLink;
            coupon.Description = affiliateCoupon.Description;
            coupon.Discount = affiliateCoupon.Discount;
            coupon.FriendlyDescription = affiliateCoupon.FriendlyDescription;
            coupon.IsPercentage = affiliateCoupon.IsPercentage;
            coupon.New = affiliateCoupon.New;
            coupon.Remark = affiliateCoupon.Remark;
            coupon.Shipping = affiliateCoupon.Shipping;
            coupon.Validity = affiliateCoupon.Validity.ToUniversalTime();

            var matchedCategory = categoriesMatches?.FirstOrDefault(matched =>
                matched.AffiliateProgram == affiliateCoupon.AffiliateProgram &&
                matched.AffiliateCategoryId == affiliateCoupon.Category.Id);

            coupon.Category = (affiliateCoupon.Category == null || matchedCategory == null)
                ? Category.GetDefaultCategory()
                : new Category
                {
                    Id = matchedCategory.AdvertiseCategoryId,
                    Name = affiliateCoupon.Category.Name,
                    FriendlyName = affiliateCoupon.Category.FriendlyName
                };

            var matchedStore = storesMatches?.FirstOrDefault(matched =>
                matched.AffiliateProgram == affiliateCoupon.AffiliateProgram &&
                matched.AffiliateStoreId == affiliateCoupon.Store.Id);

            coupon.Store = (affiliateCoupon.Store == null || matchedStore == null)
                ? Store.GetDefaultStore()
                : new Store
                {
                    Id = matchedStore.AdvertiseStoreId,
                    Name = affiliateCoupon.Store.Name,
                    FriendlyName = affiliateCoupon.Store.FriendlyName,
                    StoreUrl = affiliateCoupon.Store.StoreUrl,
                    ImageUrl = affiliateCoupon.Store.ImageUrl
                };
        }

        public void CancelCoupon(AffiliateCoupon affiliateCoupon)
        {
            
        }
    }
}