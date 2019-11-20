using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives
{
    public class IncentiveResponse
    {
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("items")]
        public int Items { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("incentiveItems")]
        public IncentiveItems IncentiveItems { get; set; }
    }
}
