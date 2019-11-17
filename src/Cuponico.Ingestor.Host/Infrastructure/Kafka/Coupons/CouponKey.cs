using System.Runtime.Serialization;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka.Coupons
{
    [DataContract(Name = "CouponKey", Namespace = "br.com.cuponico.partners.coupons")]
    public class CouponKey
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "partner")]
        public string Partner { get; set; }
    }
}
