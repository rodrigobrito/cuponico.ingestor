using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee;
using Elevar.Utils;
using Newtonsoft.Json;
using AffiliateCategory = Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories.AffiliateCategory;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Categories
{
    public class LomadeeCategoryHttpRepository: IAffiliateCategoryRepository
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;
        private readonly IMapper _mapper;
        private readonly IAffiliateCouponRepository _couponRepository;

        public LomadeeCategoryHttpRepository(LomadeeHttpSettings lomadeeSettings, HttpClient client, IMapper mapper, LomadeeCouponMongoDbRepository couponRepository)
        {
            _client = client.ThrowIfNull(nameof(client));
            _lomadeeSettings = lomadeeSettings.ThrowIfNull(nameof(lomadeeSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _couponRepository = couponRepository.ThrowIfNull(nameof(couponRepository));
        }

        public async Task<IList<LomadeeCategory>> GetAllCategoriesAsync()
        {
            var responseString = await _client.GetStringAsync(_lomadeeSettings.GetAllCategoriesUri);
            var response = JsonConvert.DeserializeObject<LomadeeCategoryResponse>(responseString, _lomadeeSettings.JsonSettings);
            return response == null || !response.Categories.Any() ? new List<LomadeeCategory>() : response.Categories;
        }

        public async Task<IList<AffiliateCategory>> GetAllAsync()
        {
            var categories = await GetAllCategoriesAsync();
            var coupons = await _couponRepository.GetAllAsync();
            foreach (var category in categories)
            {
                category.CouponsCount = coupons.Count(c => c.Category != null && c.Category.Id == category.Id);
            }
            return _mapper.Map<IList<AffiliateCategory>>(categories);
        }

        public Task SaveAsync(IList<AffiliateCategory> categories)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}