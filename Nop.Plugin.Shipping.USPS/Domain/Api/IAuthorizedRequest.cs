using System.Text.Json.Serialization;

namespace Nop.Plugin.Shipping.USPS.Domain.Api;

/// <summary>
/// Represents request with authorization
/// </summary>
public interface IAuthorizedRequest
{
    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    [JsonIgnore]
    public string Token { get; set; }
}
