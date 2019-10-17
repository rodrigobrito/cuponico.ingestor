using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Categories;
using Ingestor.ConsoleHost.Partners.Lomadee.Http.Coupons.Categories;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Jobs
{
    public class LomadeeCategoriesSchedulableJob : IInvocable
    {
        private readonly LomadeeCategoryHttpRepository _httpRepository;
        private readonly LomadeeCategoryMongoDbRepository _mongodbRepository;
        public LomadeeCategoriesSchedulableJob(LomadeeCategoryHttpRepository httpRepository, LomadeeCategoryMongoDbRepository mongodbRepository)
        {
            _httpRepository = httpRepository ?? throw new ArgumentNullException(nameof(httpRepository));
            _mongodbRepository = mongodbRepository ?? throw new ArgumentNullException(nameof(mongodbRepository));
        }

        public async Task Invoke()
        {
            var lomadeeCategories = await _httpRepository.GetAllAsync();
            if (!lomadeeCategories.Any()) return;

            var categoriesToInsert = new List<LomadeeCategory>();
            var categoriesToUpdate = new List<LomadeeCategory>();
            var categoriesToDelete = new List<LomadeeCategory>();

            var localCategories = await _mongodbRepository.GetAll();
            foreach (var lomadeeCategory in lomadeeCategories)
            {
                if (lomadeeCategory == null) continue;

                var localCategory = localCategories?.FirstOrDefault(local => local.Id == lomadeeCategory.Id);
                if (localCategory == null)
                {
                    categoriesToInsert.Add(lomadeeCategory);
                }
                else
                {
                    if (!localCategory.Equals(lomadeeCategory))
                    {
                        categoriesToUpdate.Add(lomadeeCategory);
                    }
                }
            }

            if (localCategories != null)
                categoriesToDelete.AddRange(localCategories.Where(localCategory => lomadeeCategories.All(lomadee => lomadee.Id != localCategory.Id)));

            if (categoriesToInsert.Any())
                await _mongodbRepository.SaveAsync(categoriesToInsert);

            if (categoriesToUpdate.Any())
                await _mongodbRepository.SaveAsync(categoriesToUpdate);

            if (categoriesToUpdate.Any())
                await _mongodbRepository.DeleteAsync(categoriesToUpdate.Select(x => x.Id).ToList());
        }
    }
}