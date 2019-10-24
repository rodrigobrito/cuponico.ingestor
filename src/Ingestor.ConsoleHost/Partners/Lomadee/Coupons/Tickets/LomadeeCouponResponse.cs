using System.Collections.Generic;
using Ingestor.ConsoleHost.Partners.Lomadee.Http;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Tickets
{
    public class LomadeeCouponResponse : LomadeeBaseResponse
    {
        [JsonProperty("pagination")]
        internal LomadeePagination Pagination { get; set; }

        [JsonProperty("coupons")]
        internal IList<LomadeeCoupon> Coupons { get; set; } = new List<LomadeeCoupon>();
    }
}