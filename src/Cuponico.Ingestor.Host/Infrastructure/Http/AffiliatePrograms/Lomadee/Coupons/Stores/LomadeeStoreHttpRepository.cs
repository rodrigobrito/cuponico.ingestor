using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Lomadee;
using Elevar.Utils;
using Newtonsoft.Json;
using AffiliateStore = Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores.AffiliateStore;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Stores
{
    public class LomadeeStoreHttpRepository : IAffiliateStoreRepository
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;
        private readonly IMapper _mapper;
        private readonly IAffiliateCouponRepository _couponRepository;


        public LomadeeStoreHttpRepository(LomadeeHttpSettings lomadeeSettings, HttpClient client, IMapper mapper, LomadeeCouponMongoDbRepository couponRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _lomadeeSettings = lomadeeSettings ?? throw new ArgumentNullException(nameof(lomadeeSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _couponRepository = couponRepository.ThrowIfNull(nameof(couponRepository));
        }

        internal async Task<IList<LomadeeStore>> GetAllLomadeeStoresAsync()
        {
            var responseString = await _client.GetStringAsync(_lomadeeSettings.GetAllStoresUri);

            var response = JsonConvert.DeserializeObject<LomadeeStoreResponse>(responseString, _lomadeeSettings.JsonSettings);
            if (response == null || !response.Stores.Any())
                return new List<LomadeeStore>();

            return response.Stores;
        }

        public async Task<IList<AffiliateStore>> GetAllAsync()
        {
            var stores = await GetAllLomadeeStoresAsync();
            var coupons = await _couponRepository.GetAllAsync();
            foreach (var lomadeeStore in stores)
            {
                lomadeeStore.CouponsCount = coupons.Count(c => c.Store != null && c.Store.Id == lomadeeStore.Id);
            }
            return _mapper.Map<IList<AffiliateStore>>(stores);
        }

        public Task SaveAsync(IList<AffiliateStore> stores)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}
