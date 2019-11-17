using AutoMapper;
using Cuponico.Ingestor.Host.Domain.Tickets;
using Elevar.Collections;
using Elevar.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Tickets
{
    public class LomadeeeCouponHttpRepository : ICouponRepository
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;
        private readonly IMapper _mapper;

        public LomadeeeCouponHttpRepository(LomadeeHttpSettings lomadeeSettings, HttpClient client, IMapper mapper)
        {
            _client = client.ThrowIfNull(nameof(client));
            _lomadeeSettings = lomadeeSettings.ThrowIfNull(nameof(lomadeeSettings));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        private async Task<IList<LomadeeCoupon>> GetAllLomadeeCouponsAsync()
        {
            var responseString = await _client.GetStringAsync(_lomadeeSettings.GetAllCouponsUri);

            var response = JsonConvert.DeserializeObject<LomadeeCouponResponse>(responseString,  new JsonSerializerSettings
            {
                Culture = new CultureInfo("pt-BR"),
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.DateTime,
                Error = (se, ev) => { ev.ErrorContext.Handled = true; }
            });

            if (response == null || !response.Coupons.Any())
                return new PagedList<LomadeeCoupon>();

            foreach (var coupon in response.Coupons)
            {
                coupon.UpdateProperties();
            }
            return response.Coupons;
        }

        public async Task<IList<Coupon>> GetAllAsync()
        {
            var lomadeeCoupons = await GetAllLomadeeCouponsAsync();
            return _mapper.Map<IList<Coupon>>(lomadeeCoupons);
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(IList<Coupon> coupons)
        {
            throw new System.NotImplementedException();
        }
    }
}
