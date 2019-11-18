using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Cuponico.Ingestor.Host.Domain.Categories;

namespace Cuponico.Ingestor.Host.Domain.Jobs
{
    public class CategoriesSchedulableJob : IInvocable
    {
        private readonly ICategoryRepository _repositoryFromPartner;
        private readonly ICategoryRepository _cuponicoRepository;
        public CategoriesSchedulableJob(ICategoryRepository repositoryFromPartner, ICategoryRepository cuponicoRepository)
        {
            _repositoryFromPartner = repositoryFromPartner ?? throw new ArgumentNullException(nameof(repositoryFromPartner));
            _cuponicoRepository = cuponicoRepository ?? throw new ArgumentNullException(nameof(cuponicoRepository));
        }

        public async Task Invoke()
        {
            var categoriesFromPartner = await _repositoryFromPartner.GetAllAsync();
            if (!categoriesFromPartner.Any()) return;

            var categoriesToCreate = new List<Category>();
            var categoriesToChange = new List<Category>();
            var categoriesToCancel = new List<Category>();

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
                await _cuponicoRepository.SaveAsync(categoriesToCreate);

            if (categoriesToChange.Any())
                await _cuponicoRepository.SaveAsync(categoriesToChange);

            if (categoriesToCancel.Any())
                await _cuponicoRepository.DeleteAsync(categoriesToCancel.Select(x => x.CategoryId).ToList());
        }
    }
}