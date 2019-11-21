using System;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias
{
    public class ZanoxAdmediaProgram
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("$")]
        public string OriginalName { get; set; }

        public string Name
        {
            get
            {
                var originalName = OriginalName;
                var endName = originalName.Substring(originalName.Length - 2);
                return endName == "BR" ? originalName.Substring(0, originalName.Length - 2) : originalName;
            }
        }
        public string FriendlyName => Name.ToFriendlyName();
        public string Description { get; set; }
        public Uri Uri { get; set; }
        public Uri ImageUri { get; set; }
    }
}