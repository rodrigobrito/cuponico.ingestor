using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Partners.Lomadee
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
