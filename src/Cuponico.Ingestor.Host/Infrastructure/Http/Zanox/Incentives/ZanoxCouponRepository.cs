using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Tickets;
using Elevar.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Incentives
{
    public class ZanoxCouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;
        private readonly IMapper _mapper;

        public ZanoxCouponRepository(ZanoxHttpSettings zanoxSettings, HttpClient client, IMapper mapper)
        {
            _client = client.ThrowIfNull(nameof(client));
            _zanoxSettings = zanoxSettings.ThrowIfNull(nameof(zanoxSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<IList<Coupon>> GetAllAsync()
        {
            var response = await GetAllCouponsAsync();
            return _mapper.Map<IList<Coupon>>(response.IncentiveItems.Items);
        }

        public Task SaveAsync(IList<Coupon> coupons)
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
