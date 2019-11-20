using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Categories;
using Cuponico.Ingestor.Host.Domain.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Incentives;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Zanox;
using Elevar.Utils;
using Category = Cuponico.Ingestor.Host.Domain.Categories.Category;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Programs
{
    public class ZanoxCategoryHttpRepository : ICategoryRepository
    {
        private readonly ZanoxProgramHttpRepository _programRepository;
        private readonly ZanoxStoreMongoDbRepository _storeRepository;
        private readonly ICouponRepository _couponRepository;

        public ZanoxCategoryHttpRepository(ZanoxProgramHttpRepository programRepository, ZanoxStoreMongoDbRepository storeRepository, ZanoxCouponRepository couponRepository)
        {
            _programRepository = programRepository.ThrowIfNull(nameof(programRepository));
            _storeRepository = storeRepository.ThrowIfNull(nameof(storeRepository));
            _couponRepository = couponRepository.ThrowIfNull(nameof(couponRepository));
        }

        public async Task<IList<Category>> GetAllAsync()
        {
            var coupons = await _couponRepository.GetAllAsync();

            return new List<Category>
            {
                new Category
                {
                    CategoryId = 117979,
                    Name = "Black Friday",
                    FriendlyName = "Black Friday".ToFriendlyName(),
                    ChangedDate = new DateTime(2019,11,19),
                    CouponsCount = coupons.Count
                }
            };
        }

        private async Task<IList<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();
            var stores = await _storeRepository.GetAllAsync();
            Parallel.ForEach(stores, new ParallelOptions { MaxDegreeOfParallelism = 20 }, store =>
            {
                var programRespnse = _programRepository.GetProgramAsync(store.StoreId.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                var programs = programRespnse?.Programs;
                if (programs == null) return;

                foreach (var program in programs)
                {
                    var zanoxCategories = program?.Categories;
                    if (zanoxCategories == null) continue;
                    foreach (var wrapper in zanoxCategories)
                    {
                        var zanoxSubCategories = wrapper.Category;
                        foreach (var zanoxCategory in zanoxSubCategories)
                        {
                            var category = new Category
                            {
                                CategoryId = zanoxCategory.Id,
                                Name = zanoxCategory.Name,
                                FriendlyName = zanoxCategory.Name.ToFriendlyName()
                            };
                            categories.Add(category);
                        }
                    }
                }
            });
            return categories.GroupBy(c => c.CategoryId).Select(g => g.FirstOrDefault()).ToList();
        }

        public Task SaveAsync(IList<Category> categories)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}
