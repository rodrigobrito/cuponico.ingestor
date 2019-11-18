using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Programs
{
    public class ZanoxProgram
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("adrank")]
        public decimal Rank { get; set; }
        
        [JsonProperty("applicationRequired")]
        public bool ApplicationRequired { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptionLocal")]
        public string DescriptionLocal { get; set; }

        [JsonProperty("products")]
        public long Products { get; set; }

        [JsonProperty("categories")]
        public IList<ZanoxCategory> Categories { get; set; } = new List<ZanoxCategory>();

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }
    }
}