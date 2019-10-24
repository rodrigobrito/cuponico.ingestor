using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Stores
{
    internal class LomadeeStoreResponse : LomadeeBaseResponse
    {
        [JsonProperty("pagination")]
        internal LomadeePagination Pagination { get; set; }

        [JsonProperty("stores")]
        internal IList<LomadeeStore> Stores { get; set; } = new List<LomadeeStore>();
    }
}