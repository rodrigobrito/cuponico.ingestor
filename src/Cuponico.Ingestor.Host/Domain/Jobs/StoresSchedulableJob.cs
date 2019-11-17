using Coravel.Invocable;
using Cuponico.Ingestor.Host.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Jobs
{
    public class StoresSchedulableJob : IInvocable
    {
        private readonly IStoreRepository _repositoryFromPartner;
        private readonly IStoreRepository _cuponicoRepository;
        public StoresSchedulableJob(IStoreRepository repositoryFromPartner, IStoreRepository cuponicoRepository)
        {
            _repositoryFromPartner = repositoryFromPartner ?? throw new ArgumentNullException(nameof(repositoryFromPartner));
            _cuponicoRepository = cuponicoRepository ?? throw new ArgumentNullException(nameof(cuponicoRepository));
        }

        public async Task Invoke()
        {
            var storesFromPartner = await _repositoryFromPartner.GetAllAsync();
            if (!storesFromPartner.Any()) return;

            var storesToCreate = new List<Store>();
            var storesToChange = new List<Store>();
            var storesToCancel = new List<Store>();

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
                await _cuponicoRepository.SaveAsync(storesToCreate);

            if (storesToChange.Any())
                await _cuponicoRepository.SaveAsync(storesToChange);

            if (storesToCancel.Any())
                await _cuponicoRepository.DeleteAsync(storesToCancel.Select(x => x.StoreId).ToList());
        }
    }
}