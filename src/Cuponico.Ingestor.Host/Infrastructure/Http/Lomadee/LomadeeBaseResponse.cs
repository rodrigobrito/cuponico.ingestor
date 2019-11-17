using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee
{
    public abstract class LomadeeBaseResponse
    {
        [JsonProperty("requestInfo")]
        internal LomadeeRequestInfo RequestInfo { get; set; }
    }
}
