using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Stores
{
    public class LomadeeStoreHttpRepository 
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;

        public LomadeeStoreHttpRepository(LomadeeHttpSettings lomadeeSettings, HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _lomadeeSettings = lomadeeSettings ?? throw new ArgumentNullException(nameof(lomadeeSettings));
        }

        public async Task<IList<LomadeeStore>> GetAllAsync()
        {
            var responseString = await _client.GetStringAsync(_lomadeeSettings.GetAllStoresUri);

            var response = JsonConvert.DeserializeObject<LomadeeStoreResponse>(responseString, _lomadeeSettings.JsonSettings);
            if (response == null || !response.Stores.Any())
                return new List<LomadeeStore>();

            return response.Stores;
        }
    }
}
