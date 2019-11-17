using System;
using System.Runtime.Serialization;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka.Coupons
{
    [DataContract(Name = "Store", Namespace = "br.com.cuponico.partners.coupons")]
    public class Store
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "image")]
        public Uri Image { get; set; }
        [DataMember(Name = "link")]
        public Uri Link { get; set; }
    }
}
