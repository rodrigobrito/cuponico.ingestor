using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxAdmediaProgram
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("$")]
        public string Name { get; set; }
        public string FriendlyName => Name.ToFriendlyName();
        public string Description { get; set; }
    }
}