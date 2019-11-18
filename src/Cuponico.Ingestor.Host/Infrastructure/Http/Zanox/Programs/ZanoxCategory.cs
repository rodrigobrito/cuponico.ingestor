using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Programs
{
    public class ZanoxCategory
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("$")]
        public long Name { get; set; }
    }
}
