using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives
{
    public class IncentiveProgram
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("$")]
        public string Name { get; set; }
        public string FriendelyName => Name.ToFriendlyName();
    }
}