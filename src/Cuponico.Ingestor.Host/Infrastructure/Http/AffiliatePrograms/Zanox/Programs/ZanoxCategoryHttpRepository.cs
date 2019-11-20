using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox;
using Elevar.Utils;
using AffiliateCategory = Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories.AffiliateCategory;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Programs
{
    public class ZanoxCategoryHttpRepository : IAffiliateCategoryRepository
    {
        private readonly ZanoxProgramHttpRepository _programRepository;
        private readonly ZanoxStoreMongoDbRepository _storeRepository;
        private readonly IAffiliateCouponRepository _couponRepository;

        public ZanoxCategoryHttpRepository(ZanoxProgramHttpRepository programRepository, ZanoxStoreMongoDbRepository storeRepository, ZanoxCouponRepository couponRepository)
        {
            _programRepository = programRepository.ThrowIfNull(nameof(programRepository));
            _storeRepository = storeRepository.ThrowIfNull(nameof(storeRepository));
            _couponRepository = couponRepository.ThrowIfNull(nameof(couponRepository));
        }

        public async Task<IList<AffiliateCategory>> GetAllAsync()
        {
            var coupons = await _couponRepository.GetAllAsync();

            return new List<AffiliateCategory>
            {
                new AffiliateCategory
                {
                    CategoryId = 117979,
                    AffiliateProgram = Affiliates.Zanox,
                    Name = "Black Friday",
                    FriendlyName = "Black Friday".ToFriendlyName(),
                    ChangedDate = new DateTime(2019,11,19),
                    CouponsCount = coupons.Count
                }
            };
        }

        private async Task<IList<AffiliateCategory>> GetAllCategoriesAsync()
        {
            var categories = new List<AffiliateCategory>();
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
                            var category = new AffiliateCategory
                            {
                                CategoryId = zanoxCategory.Id,
                                AffiliateProgram = Affiliates.Zanox,
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

        public Task SaveAsync(IList<AffiliateCategory> categories)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}
