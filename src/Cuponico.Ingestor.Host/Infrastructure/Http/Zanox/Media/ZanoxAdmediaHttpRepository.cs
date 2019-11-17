using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Media
{
    public class ZanoxAdmediaHttpRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;

        public ZanoxAdmediaHttpRepository(ZanoxHttpSettings zanoxSettings, HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _zanoxSettings = zanoxSettings ?? throw new ArgumentNullException(nameof(zanoxSettings));
        }

        public async Task<ZanoxAdmediaResponse> GetAllStartPageMediaAsync(int page = 0)
        {
            var responseString = await _client.GetStringAsync($"{_zanoxSettings.GetStartpageMediaUri}&items=50&page={page}");
            return JsonConvert.DeserializeObject<ZanoxAdmediaResponse>(responseString, _zanoxSettings.JsonSettings);
        }
    }
}