using Newtonsoft.Json;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Http
{
    internal class LomadeePagination
    {
        [JsonProperty("page")]
        internal long Page { get; set; }

        [JsonProperty("size")]
        internal long Size { get; set; }

        [JsonProperty("totalSize")]
        internal long TotalSize { get; set; }

        [JsonProperty("totalPage")]
        internal long TotalPage { get; set; }
    }
}
