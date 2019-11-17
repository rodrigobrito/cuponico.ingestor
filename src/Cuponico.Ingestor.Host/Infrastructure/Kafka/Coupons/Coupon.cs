using System;
using System.Runtime.Serialization;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka.Coupons
{
    [DataContract(Name = "Coupon", Namespace = "br.com.cuponico.partners.coupons")]
    public class Coupon
    {
        public CouponKey Key => new CouponKey { Id = Id, Partner = Partner };

        [DataMember(Name = "partner")]
        public string Partner { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "friendlyDescription")]
        public string FriendlyDescription { get; set; }

        [DataMember(Name = "remark")]
        public string Remark { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "discount")]
        public decimal Discount { get; set; }

        [DataMember(Name = "store")]
        public Store Store { get; set; }

        [DataMember(Name = "category")]
        public Category Category { get; set; }

        [DataMember(Name = "vigency")]
        public DateTime Vigency { get; set; }

        [DataMember(Name = "link")]
        public Uri Link { get; set; }

        [DataMember(Name = "new")]
        public bool New { get; set; }
    }
}
