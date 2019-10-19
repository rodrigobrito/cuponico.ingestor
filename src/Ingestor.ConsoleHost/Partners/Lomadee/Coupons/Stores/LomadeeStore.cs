using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Stores
{
    public class LomadeeStore
    {
        [BsonId]
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }
    }
}
