using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Media
{
    public class ZanoxAdmediaProgram
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("$")]
        public string Name { get; set; }
    }
}