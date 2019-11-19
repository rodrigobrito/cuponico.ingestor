using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias
{
    public class ZanoxTrackinsLinks
    {
        [JsonProperty("trackingLink")]
        public IList<ZanoxTrackingLink> Links { get; set; } = new List<ZanoxTrackingLink>();
        public Uri Url => Links.FirstOrDefault()?.Uri;
        public Uri ImageUrl => Links.FirstOrDefault()?.ImageUri;
    }
}