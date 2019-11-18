using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxAdmediaItems
    {
        [JsonProperty("admediumItem")]
        public IList<ZanoxAdmedia> Items { get; set; } = new List<ZanoxAdmedia>();
    }
}
