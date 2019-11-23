using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Lomadee;
using Elevar.Collections;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Tickets
{
    public class LomadeeeCouponHttpRepository : IAffiliateCouponRepository
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

        public async Task<IList<AffiliateCoupon>> GetAllAsync()
        {
            var lomadeeCoupons = await GetAllLomadeeCouponsAsync();
            return _mapper.Map<IList<AffiliateCoupon>>(lomadeeCoupons);
        }

        public Task DeleteAsync(IList<long> ids)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(IList<AffiliateCoupon> coupons)
        {
            throw new System.NotImplementedException();
        }
    }
}
