using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Advertiser.Stores;
using Elevar.Utils;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public class AffiliateStoreDomainService
    {
        private readonly IAffiliateStoreMatchesRepository _matchesRepository;
        private readonly IStoreRepository _storeRepository;
        public AffiliateStoreDomainService(IAffiliateStoreMatchesRepository matchesRepository, IStoreRepository storeRepository)
        {
            _matchesRepository = matchesRepository.ThrowIfNull(nameof(matchesRepository));
            _storeRepository = storeRepository.ThrowIfNull(nameof(storeRepository));
        }

        public async Task ProcessUnifiedStore(AffiliateStore affiliateStore)
        {
            if (affiliateStore == null)
                throw new ArgumentNullException(nameof(affiliateStore));

            var matchedIds = await _matchesRepository.GetAllAsync();
            var stores = await _storeRepository.GetAllAsync();

            var updated = await UpdateStoreIfAlreadyExistsInTheSameAffiliateProgram(stores, matchedIds, affiliateStore);
            if (updated) return;

            updated = await UpdateStoreIfAlreadyExistsInOtherAffiliateProgram(stores, matchedIds, affiliateStore);
            if (updated) return;

            await CreateStoreAndMatch(affiliateStore);
        }

        private async Task<bool> UpdateStoreIfAlreadyExistsInTheSameAffiliateProgram(
            IList<Store> existingStores,
            IList<AffiliateStoreMatch> existingMatches,
            AffiliateStore affiliateStore)
        {
            if (existingStores == null || !existingStores.Any()) return false;
            if (existingMatches == null || !existingMatches.Any()) return false;

            var advertiseId = affiliateStore.GetAdvertiseId();
            var previousMatch = existingMatches.FirstOrDefault(id => id.Matched(advertiseId));
            if (previousMatch == null) return false;

            var foundStore = existingStores.FirstOrDefault(store => store.StoreId == previousMatch.AdvertiseStoreId);
            if (foundStore == null) return false;

            // Update match
            previousMatch.Update(advertiseId);
            await _matchesRepository.SaveAsync(previousMatch);

            // Update store
            UpdateStoreProperties(foundStore, affiliateStore, existingMatches);
            await _storeRepository.SaveAsync(foundStore);

            return true;
        }

        private async Task<bool> UpdateStoreIfAlreadyExistsInOtherAffiliateProgram(
            IList<Store> existingStores,
            IList<AffiliateStoreMatch> existingMatches,
            AffiliateStore affiliateStore)
        {
            if (existingStores == null || !existingStores.Any()) return false;
            if (existingMatches == null || !existingMatches.Any()) return false;

            var storeToUpdate = existingStores.FirstOrDefault(store => store.IsMatchableName(affiliateStore.Name));
            if (storeToUpdate == null) return false;

            var newMatch = AffiliateStoreMatch.Create(storeToUpdate, affiliateStore);
            await _matchesRepository.SaveAsync(newMatch);
            UpdateStoreProperties(storeToUpdate, affiliateStore, existingMatches);
            await _storeRepository.SaveAsync(storeToUpdate);
            return true;
        }
        private async Task CreateStoreAndMatch(AffiliateStore affiliateStore)
        {
            var storeToCreate = Store.Create();
            UpdateStoreProperties(storeToCreate, affiliateStore);
            await _storeRepository.SaveAsync(storeToCreate);
            var newMatch = AffiliateStoreMatch.Create(storeToCreate, affiliateStore);
            await _matchesRepository.SaveAsync(newMatch);
        }

        private int GetMaxAcceptableMatchDistance(string name)
        {
            if (name.Length <= 6) return 1;
            return (name.Length * 1) / 6;
        }

        private static void UpdateStoreProperties(Store store, AffiliateStore affiliateStore, IList<AffiliateStoreMatch> allMatches = null)
        {
            var advertiseId = affiliateStore.GetAdvertiseId();

            store.Name = affiliateStore.Name;
            store.FriendlyName = affiliateStore.FriendlyName;
            store.Description = string.IsNullOrWhiteSpace(affiliateStore.Description) && !string.IsNullOrWhiteSpace(store.Description) ? store.Description : affiliateStore.Description;
            store.ImageUrl = affiliateStore.ImageUrl;
            store.StoreUrl = affiliateStore.StoreUrl;
            store.ChangedDate = affiliateStore.ChangedDate;
            if (allMatches == null || !allMatches.Any())
                store.CouponsCount = affiliateStore.CouponsCount;
            else
                store.CouponsCount = affiliateStore.CouponsCount + allMatches.Count(m => !m.Matched(advertiseId) && m.AdvertiseStoreId == store.StoreId);
        }
    }
}
