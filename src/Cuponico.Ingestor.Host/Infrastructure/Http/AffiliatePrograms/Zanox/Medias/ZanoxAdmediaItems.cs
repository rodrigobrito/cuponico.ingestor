using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias
{
    public class ZanoxAdmediaItems
    {
        [JsonProperty("admediumItem")]
        [JsonConverter(typeof(SafeCollectionConverter))]
        public IList<ZanoxAdmedia> Items { get; set; } = new List<ZanoxAdmedia>();
    }
}
