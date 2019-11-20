using System;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias
{
    public class ZanoxTrackingLink
    {
        [JsonProperty("@adspaceId")]
        public string Id { get; set; }

        [JsonProperty("ppv")]
        public Uri ImageUri { get; set; }

        [JsonProperty("ppc")]
        public Uri Uri { get; set; }
    }
}
