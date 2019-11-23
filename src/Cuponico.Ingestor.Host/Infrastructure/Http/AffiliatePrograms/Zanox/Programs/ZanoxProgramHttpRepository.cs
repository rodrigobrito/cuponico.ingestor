using System.Net.Http;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Zanox;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Programs
{
    public class ZanoxProgramHttpRepository
    {
        private readonly HttpClient _client;
        private readonly ZanoxHttpSettings _zanoxSettings;

        public ZanoxProgramHttpRepository(ZanoxHttpSettings zanoxSettings, HttpClient client)
        {
            _client = client.ThrowIfNull(nameof(client));
            _zanoxSettings = zanoxSettings.ThrowIfNull(nameof(zanoxSettings));
        }

        public async Task<ZanoxProgramResponse> GetProgramAsync(string programId)
        {
            var responseString = await _client.GetStringAsync(_zanoxSettings.GetProgramUri(programId));
            return JsonConvert.DeserializeObject<ZanoxProgramResponse>(responseString, _zanoxSettings.JsonSettings);
        }
    }
}
