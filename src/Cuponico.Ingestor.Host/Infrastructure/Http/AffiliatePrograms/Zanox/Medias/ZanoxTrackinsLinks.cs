using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias
{
    public class ZanoxTrackinsLinks
    {
        [JsonProperty("trackingLink")]
        public IList<ZanoxTrackingLink> Links { get; set; } = new List<ZanoxTrackingLink>();

        public Uri Url
        {
            get
            {
                var uri = Links?.FirstOrDefault()?.Uri?.ToString();
                return uri == null ? null : new Uri(uri.ToLower().Replace("http://", "https://"));
            }
        }

        public Uri ImageUrl => Links.FirstOrDefault()?.ImageUri;
    }
}