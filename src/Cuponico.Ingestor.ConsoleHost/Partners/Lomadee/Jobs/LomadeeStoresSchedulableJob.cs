using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Stores;

namespace Cuponico.Ingestor.Host.Partners.Lomadee.Jobs
{
    public class LomadeeStoresSchedulableJob : IInvocable
    {
        private readonly LomadeeStoreHttpRepository _httpRepository;
        private readonly LomadeeStoreMongoDbRepository _mongodbRepository;
        public LomadeeStoresSchedulableJob(LomadeeStoreHttpRepository httpRepository, LomadeeStoreMongoDbRepository mongodbRepository)
        {
            _httpRepository = httpRepository ?? throw new ArgumentNullException(nameof(httpRepository));
            _mongodbRepository = mongodbRepository ?? throw new ArgumentNullException(nameof(mongodbRepository));
        }

        public async Task Invoke()
        {
            var lomadeeStores = await _httpRepository.GetAllAsync();
            if (!lomadeeStores.Any()) return;

            var storesToInsert = new List<LomadeeStore>();
            var storesToUpdate = new List<LomadeeStore>();
            var storesToDelete = new List<LomadeeStore>();

            var localStores = await _mongodbRepository.GetAll();
            foreach (var lomadeeStore in lomadeeStores)
            {
                if (lomadeeStore == null) continue;

                var localStore = localStores?.FirstOrDefault(local => local.Id == lomadeeStore.Id);
                if (localStore == null)
                {
                    storesToInsert.Add(lomadeeStore);
                }
                else
                {
                    if (!localStore.Equals(lomadeeStore))
                    {
                        storesToUpdate.Add(lomadeeStore);
                    }
                }
            }

            if (localStores != null)
                storesToDelete.AddRange(localStores.Where(localStore => lomadeeStores.All(lomadee => lomadee.Id != localStore.Id)));

            if (storesToInsert.Any())
                await _mongodbRepository.SaveAsync(storesToInsert);

            if (storesToUpdate.Any())
                await _mongodbRepository.SaveAsync(storesToUpdate);

            if (storesToDelete.Any())
                await _mongodbRepository.DeleteAsync(storesToDelete.Select(x => x.Id).ToList());
        }
    }
}