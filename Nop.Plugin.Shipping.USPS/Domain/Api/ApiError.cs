using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api;

public class ApiError : ApiResponse
{
    [JsonProperty("apiVersion")]
    public string ApiVersion { get; set; }

    [JsonProperty("error")]
    public ErrorSummary Error { get; set; }

    #region Nested classes

    public class ErrorSummary
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("errors")]
        public List<ErrorInfo> Errors { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class ErrorInfo
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("source")]
        public object Source { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    #endregion
}


