using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Collections;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs
{
    public class AffiliateStoresSchedulableJob : IInvocable
    {
        private readonly IAffiliateStoreRepository _repositoryFromPartner;
        private readonly IAffiliateStoreRepository _cuponicoRepository;
        private readonly IPublisher _publisher;
        public AffiliateStoresSchedulableJob(IAffiliateStoreRepository repositoryFromPartner, IAffiliateStoreRepository cuponicoRepository, IPublisher publisher)
        {
            _repositoryFromPartner = repositoryFromPartner ?? throw new ArgumentNullException(nameof(repositoryFromPartner));
            _cuponicoRepository = cuponicoRepository ?? throw new ArgumentNullException(nameof(cuponicoRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task Invoke()
        {
            var storesFromPartner = await _repositoryFromPartner.GetAllAsync();
            if (!storesFromPartner.Any()) return;

            var storesToCreate = new List<AffiliateStore>();
            var storesToChange = new List<AffiliateStore>();
            var storesToCancel = new List<AffiliateStore>();

            var cuponicoStores = await _cuponicoRepository.GetAllAsync();
            foreach (var partnerStore in storesFromPartner)
            {
                if (partnerStore == null) continue;

                var cuponicoStore = cuponicoStores?.FirstOrDefault(local => local.StoreId == partnerStore.StoreId);
                if (cuponicoStore == null)
                {
                    storesToCreate.Add(partnerStore);
                }
                else
                {
                    if (!cuponicoStore.Equals(partnerStore))
                    {
                        storesToChange.Add(partnerStore);
                    }
                }
            }

            if (cuponicoStores != null)
                storesToCancel.AddRange(cuponicoStores.Where(localStore => storesFromPartner.All(s => s.StoreId != localStore.StoreId)));

            if (storesToCreate.Any())
            {
                var baches = storesToCreate.BatchesOf(50);
                foreach (var bach in baches)
                {
                    var stores = bach.ToList();
                    await _cuponicoRepository.SaveAsync(stores);
                    var createdStores = AffiliateStoreCreated.CreateMany(stores);
                    await _publisher.PublishAsync(createdStores);
                }
            }

            if (storesToChange.Any())
            {
                var baches = storesToChange.BatchesOf(50);
                foreach (var bach in baches)
                {
                    var stores = bach.ToList();
                    await _cuponicoRepository.SaveAsync(stores.ToList());
                    var changedStores = AffiliateStoreChanged.CreateMany(stores);
                    await _publisher.PublishAsync(changedStores);
                }
            }

            if (storesToCancel.Any())
            {
                var baches = storesToCancel.BatchesOf(50);
                foreach (var bach in baches)
                {
                    var stores = bach.ToList();
                    await _cuponicoRepository.DeleteAsync(stores.Select(x => x.StoreId).ToList());
                    var changedStores = AffiliateStoreCanceled.CreateMany(stores);
                    await _publisher.PublishAsync(changedStores);
                }
            }
        }

    }
}