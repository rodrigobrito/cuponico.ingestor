using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxAdmedia
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Adrank")]
        public int Rank { get; set; }

        [JsonProperty("admediumType")]
        public string Type { get; set; }

        [JsonProperty("program")]
        public ZanoxAdmediaProgram Program { get; set; }

        [JsonProperty("category")]
        public ZanoxAdmediaProgram Category { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("trackingLinks")]
        public ZanoxTrackinsLinks Tracking { get; set; }
    }
}
