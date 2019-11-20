using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Tickets
{
    public class LomadeeCouponResponse : LomadeeBaseResponse
    {
        [JsonProperty("pagination")]
        internal LomadeePagination Pagination { get; set; }

        [JsonProperty("coupons")]
        internal IList<LomadeeCoupon> Coupons { get; set; } = new List<LomadeeCoupon>();
    }
}