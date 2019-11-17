using Cuponico.Ingestor.Host.Domain.Stores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Lomadee;
using Elevar.Utils;
using Store = Cuponico.Ingestor.Host.Domain.Stores.Store;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Stores
{
    public class LomadeeStoreHttpRepository : IStoreRepository
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;


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

        public async Task<IList<Store>> GetAllAsync()
        {
            var stores = await GetAllLomadeeStoresAsync();
            var coupons = await _couponRepository.GetAllAsync();
            foreach (var lomadeeStore in stores)
            {
                lomadeeStore.CouponsCount = coupons.Count(c => c.Store != null && c.Store.Id == lomadeeStore.Id);
            }
            return _mapper.Map<IList<Store>>(stores);
        }

        public Task SaveAsync(IList<Store> stores)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }
    }
}
