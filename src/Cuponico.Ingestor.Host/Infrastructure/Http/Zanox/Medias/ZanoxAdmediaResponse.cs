using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxAdmediaResponse
    {
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("items")]
        public int Items { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("admediumItems")]
        public ZanoxAdmediaItems Admedium { get; set; }
    }
}