using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Elevar.Collections;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Tickets
{
    public class LomadeeeCouponHttpRepository 
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;

        public LomadeeeCouponHttpRepository(LomadeeHttpSettings lomadeeSettings, HttpClient client)
        {
            _client = client.ThrowIfNull(nameof(client));
            _lomadeeSettings = lomadeeSettings.ThrowIfNull(nameof(lomadeeSettings));
        }

        public async Task<IList<LomadeeCoupon>> GetAllAsync()
        {
            var responseString = await _client.GetStringAsync(_lomadeeSettings.GetAllCouponsUri);

            var response = JsonConvert.DeserializeObject<LomadeeCouponResponse>(responseString, _lomadeeSettings.JsonSettings);
            if (response == null || !response.Coupons.Any())
                return new PagedList<LomadeeCoupon>();

            foreach (var coupon in response.Coupons)
            {
                coupon.UpdateProperties();
            }

            return response.Coupons;
        }
    }
}
