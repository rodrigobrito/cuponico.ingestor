using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Programs
{
    public class ZanoxCategory
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("$")]
        public string Name { get; set; }
    }

    public class ZanoxCategoryWrapper
    {
        [JsonProperty("category")]
        [JsonConverter(typeof(SafeCollectionConverter))]
        public IList<ZanoxCategory> Category { get; set; } = new List<ZanoxCategory>();
    }
}
