using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api;

/// <summary>
/// Represents base request object
/// </summary>
public abstract class ApiRequest : IApiRequest
{
    /// <summary>
    /// Gets the request path
    /// </summary>
    [JsonIgnore]
    public abstract string Path { get; }

    /// <summary>
    /// Gets the request method
    /// </summary>
    [JsonIgnore]
    public abstract HttpMethod Method { get; }

    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    [JsonIgnore]
    public string Token { get; set; }
}
