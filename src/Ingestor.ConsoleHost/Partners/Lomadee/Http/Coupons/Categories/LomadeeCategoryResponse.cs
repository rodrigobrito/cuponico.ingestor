using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Http.Coupons.Categories
{
    internal class LomadeeCategoryResponse : LomadeeBaseResponse
    {
        [JsonProperty("pagination")]
        internal LomadeePagination Pagination { get; set; }

        [JsonProperty("categories")]
        internal IList<LomadeeCategory> Categories { get; set; } = new List<LomadeeCategory>();
    }
}