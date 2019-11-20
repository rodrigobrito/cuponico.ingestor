using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives
{
    public class ZanoxCouponRepository : IAffiliateCouponRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;
        private readonly IMapper _mapper;
        private readonly IAffiliateStoreRepository _storeRepository;

        public ZanoxCouponRepository(ZanoxHttpSettings zanoxSettings, HttpClient client, IMapper mapper, ZanoxStoreMongoDbRepository storeRepository)
        {
            _client = client.ThrowIfNull(nameof(client));
            _zanoxSettings = zanoxSettings.ThrowIfNull(nameof(zanoxSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
            _storeRepository = storeRepository.ThrowIfNull(nameof(storeRepository));
        }

        public async Task<IList<AffiliateCoupon>> GetAllAsync()
        {
            var response = await GetAllCouponsAsync();
            var coupons = _mapper.Map<IList<AffiliateCoupon>>(response.IncentiveItems.Items);
            
            var stores = await _storeRepository.GetAllAsync();
            coupons = coupons.Where(coupon => stores.Any(store => store.StoreId == coupon.Store.Id)).ToList();  // Only authorized retailers coupons.

            foreach (var coupon in coupons)
            {
                if (coupon.Store == null) continue;

                var store = stores.FirstOrDefault(s => s.StoreId == coupon.Store.Id);
                if (store == null) continue;

                coupon.Store.ImageUrl = store.ImageUrl;
                coupon.Store.StoreUrl = store.StoreUrl;
                if (!string.IsNullOrWhiteSpace(coupon.Description) && !string.IsNullOrWhiteSpace(coupon.Remark) && coupon.Description.Contains(coupon.Remark))
                    coupon.Remark = string.Empty;

                if (coupon.Category != null)
                {
                    coupon.Category.Id = 117979;
                    coupon.Category.Name = "Black Friday";
                    coupon.Category.FriendlyName = "Black Friday".ToFriendlyName();
                }
            }
            return coupons;
        }

        public Task SaveAsync(IList<AffiliateCoupon> coupons)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new NotImplementedException();
        }

        private async Task<IncentiveResponse> GetAllCouponsAsync()
        {
            var responseString = await _client.GetStringAsync(_zanoxSettings.GetAllCouponsUri);
            return JsonConvert.DeserializeObject<IncentiveResponse>(responseString, _zanoxSettings.JsonSettings);
        }
    }
}
