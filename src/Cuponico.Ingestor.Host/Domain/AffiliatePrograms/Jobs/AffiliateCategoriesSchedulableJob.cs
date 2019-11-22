using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs
{
    public class AffiliateCategoriesSchedulableJob : IInvocable
    {
        private readonly IAffiliateCategoryRepository _repositoryFromPartner;
        private readonly IAffiliateCategoryRepository _cuponicoRepository;
        private readonly IPublisher _publisher;
        public AffiliateCategoriesSchedulableJob(IAffiliateCategoryRepository repositoryFromPartner, IAffiliateCategoryRepository cuponicoRepository, IPublisher publisher)
        {
            _repositoryFromPartner = repositoryFromPartner ?? throw new ArgumentNullException(nameof(repositoryFromPartner));
            _cuponicoRepository = cuponicoRepository ?? throw new ArgumentNullException(nameof(cuponicoRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task Invoke()
        {
            var categoriesFromPartner = await _repositoryFromPartner.GetAllAsync();
            if (!categoriesFromPartner.Any()) return;

            var categoriesToCreate = new List<AffiliateCategory>();
            var categoriesToChange = new List<AffiliateCategory>();
            var categoriesToCancel = new List<AffiliateCategory>();

            var cuponicoCategories = await _cuponicoRepository.GetAllAsync();
            foreach (var partnerCategory in categoriesFromPartner)
            {
                if (partnerCategory == null) continue;

                var cuponicoCategory = cuponicoCategories?.FirstOrDefault(local => local.CategoryId == partnerCategory.CategoryId);
                if (cuponicoCategory == null)
                {
                    categoriesToCreate.Add(partnerCategory);
                }
                else
                {
                    if (!cuponicoCategory.Equals(partnerCategory))
                    {
                        categoriesToChange.Add(partnerCategory);
                    }
                }
            }

            if (cuponicoCategories != null)
                categoriesToCancel.AddRange(cuponicoCategories.Where(localCategory => categoriesFromPartner.All(c => c.CategoryId != localCategory.CategoryId)));

            if (categoriesToCreate.Any())
            {
                await _cuponicoRepository.SaveAsync(categoriesToCreate);
                var createdCategories = AffiliateCategoryCreated.CreateMany(categoriesToCreate);
                await _publisher.PublishAsync(createdCategories);
            }

            if (categoriesToChange.Any())
            {
                await _cuponicoRepository.SaveAsync(categoriesToChange);
                var changedCategories = AffiliateCategoryChanged.CreateMany(categoriesToChange);
                await _publisher.PublishAsync(changedCategories);
            }

            if (categoriesToCancel.Any())
            {
                await _cuponicoRepository.DeleteAsync(categoriesToCancel.Select(x => x.CategoryId).ToList());
                var canceledCategories = AffiliateCategoryCanceled.CreateMany(categoriesToCancel);
                await _publisher.PublishAsync(canceledCategories);
            }
        }
    }
}