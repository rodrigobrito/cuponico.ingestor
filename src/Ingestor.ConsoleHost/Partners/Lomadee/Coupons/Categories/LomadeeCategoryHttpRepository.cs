using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Elevar.Utils;
using Ingestor.ConsoleHost.Partners.Lomadee.Http.Coupons.Categories;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Categories
{
    public class LomadeeCategoryHttpRepository
    {
        private readonly HttpClient _client;
        private readonly LomadeeHttpSettings _lomadeeSettings;

        public LomadeeCategoryHttpRepository(LomadeeHttpSettings lomadeeSettings, HttpClient client)
        {
            _client = client.ThrowIfNull(nameof(client));
            _lomadeeSettings = lomadeeSettings.ThrowIfNull(nameof(lomadeeSettings));
        }

        public async Task<IList<LomadeeCategory>> GetAllAsync()
        {
            var responseString = await _client.GetStringAsync(_lomadeeSettings.GetAllCategoriesUri);
            var response = JsonConvert.DeserializeObject<LomadeeCategoryResponse>(responseString, _lomadeeSettings.JsonSettings);
            return response == null || !response.Categories.Any() ? new List<LomadeeCategory>() : response.Categories;
        }
    }
}