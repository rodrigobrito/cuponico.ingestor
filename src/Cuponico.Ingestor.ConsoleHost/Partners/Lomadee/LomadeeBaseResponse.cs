using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Partners.Lomadee
{
    public abstract class LomadeeBaseResponse
    {
        [JsonProperty("requestInfo")]
        internal LomadeeRequestInfo RequestInfo { get; set; }
    }
}
