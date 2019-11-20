using System;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives
{
    public class IncentiveItem
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("program")]
        public IncentiveProgram Program { get; set; }

        [JsonProperty("admedia")]
        public ZanoxAdmediaItems Admedia { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime ChangedDate { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("info4publisher")]
        public string PublisherInfo { get; set; }

        [JsonProperty("info4customer")]
        public string CustomerInfo { get; set; }

        [JsonProperty("couponCode")]
        public string CouponCode { get; set; }

        [JsonProperty("percentage")]
        public decimal Percentage { get; set; }

        [JsonProperty("restrictions")]
        public string Restrictions { get; set; }

        [JsonProperty("incentiveType")]
        public string IncentiveType { get; set; }

        [JsonProperty("newCustomerOnly")]
        public bool NewCustomerOnly { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }

        [JsonProperty("minimumBasketValue")]
        public decimal MinimumBasketValue { get; set; }
    }
}
