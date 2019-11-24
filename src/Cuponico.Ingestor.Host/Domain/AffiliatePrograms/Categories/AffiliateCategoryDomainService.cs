using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Advertiser.Categories;
using Elevar.Utils;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategoryDomainService
    {
        private readonly IAffiliateCategoryMatchesRepository _matchesRepository;
        private readonly ICategoryRepository _categoryRepository;
        public AffiliateCategoryDomainService(IAffiliateCategoryMatchesRepository matchesRepository, ICategoryRepository categoryRepository)
        {
            _matchesRepository = matchesRepository.ThrowIfNull(nameof(matchesRepository));
            _categoryRepository = categoryRepository.ThrowIfNull(nameof(categoryRepository));
        }

        public async Task ProcessUnifiedCategory(AffiliateCategory affiliateCategory)
        {
            if (affiliateCategory == null)
                throw new ArgumentNullException(nameof(affiliateCategory));

            var matchedIds = await _matchesRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var updated = await UpdateCategoryIfAlreadyExistsInTheSameAffiliateProgram(categories, matchedIds, affiliateCategory);
            if (updated) return;

            updated = await UpdateCategoryIfAlreadyExistsInOtherAffiliateProgram(categories, matchedIds, affiliateCategory);
            if (updated) return;

            await CreateCategoryAndMatch(affiliateCategory);
        }

        private async Task<bool> UpdateCategoryIfAlreadyExistsInTheSameAffiliateProgram(
            IList<Category> existingCategorys,
            IList<AffiliateCategoryMatch> existingMatches,
            AffiliateCategory affiliateCategory)
        {
            if (existingCategorys == null || !existingCategorys.Any()) return false;
            if (existingMatches == null || !existingMatches.Any()) return false;

            var advertiseId = affiliateCategory.GetAdvertiseId();
            var previousMatch = existingMatches.FirstOrDefault(id => id.Matched(advertiseId));
            if (previousMatch == null) return false;

            var foundCategory = existingCategorys.FirstOrDefault(category => category.CategoryId == previousMatch.AdvertiseCategoryId);
            if (foundCategory == null) return false;

            // Update match
            previousMatch.Update(advertiseId);
            await _matchesRepository.SaveAsync(previousMatch);

            // Update category
            UpdateCategoryProperties(foundCategory, affiliateCategory, existingMatches);
            await _categoryRepository.SaveAsync(foundCategory);

            return true;
        }

        private async Task<bool> UpdateCategoryIfAlreadyExistsInOtherAffiliateProgram(
            IList<Category> existingCategorys,
            IList<AffiliateCategoryMatch> existingMatches,
            AffiliateCategory affiliateCategory)
        {
            if (existingCategorys == null || !existingCategorys.Any()) return false;
            if (existingMatches == null || !existingMatches.Any()) return false;

            var categoryToUpdate = existingCategorys.FirstOrDefault(category => category.IsMatchableName(affiliateCategory.Name));
            if (categoryToUpdate == null) return false;

            var newMatch = AffiliateCategoryMatch.Create(categoryToUpdate, affiliateCategory);
            await _matchesRepository.SaveAsync(newMatch);
            UpdateCategoryProperties(categoryToUpdate, affiliateCategory, existingMatches);
            await _categoryRepository.SaveAsync(categoryToUpdate);
            return true;
        }
        private async Task CreateCategoryAndMatch(AffiliateCategory affiliateCategory)
        {
            var categoryToCreate = Category.Create();
            UpdateCategoryProperties(categoryToCreate, affiliateCategory);
            await _categoryRepository.SaveAsync(categoryToCreate);
            var newMatch = AffiliateCategoryMatch.Create(categoryToCreate, affiliateCategory);
            await _matchesRepository.SaveAsync(newMatch);
        }

        private static void UpdateCategoryProperties(Category category, AffiliateCategory affiliateCategory, IList<AffiliateCategoryMatch> allMatches = null)
        {
            var advertiseId = affiliateCategory.GetAdvertiseId();

            category.Name = affiliateCategory.Name;
            category.FriendlyName = affiliateCategory.FriendlyName;
            category.CategoryUrl = affiliateCategory.CategoryUrl;
            category.ChangedDate = affiliateCategory.ChangedDate;
            if (allMatches == null || !allMatches.Any())
                category.CouponsCount = affiliateCategory.CouponsCount;
            else
                category.CouponsCount = allMatches.Where(m => m.AdvertiseCategoryId == category.CategoryId).Sum(m => m.CouponsCount);
        }

        public void CancelUnifiedCategory(AffiliateCategory affiliateCategory)
        {

        }
    }
}
