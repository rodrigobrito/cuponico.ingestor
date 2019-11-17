using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Media
{
    public class ZanoxTrackinsLinks
    {
        [JsonProperty("trackingLink")]
        public IList<ZanoxTrackingLink> Links { get; set; } = new List<ZanoxTrackingLink>();
    }
}
