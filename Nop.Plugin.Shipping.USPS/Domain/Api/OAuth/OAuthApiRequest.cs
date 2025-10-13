using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.OAuth;

/// <summary>
/// Represents request to get access token
/// </summary>
public class OAuthApiRequest : ApiRequest
{
    /// <summary>
    /// Gets or sets the grant type
    /// </summary>
    [JsonProperty("grant_type")]
    public string GrantType { get; set; }

    /// <summary>
    /// Gets or sets the client ID
    /// </summary>
    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret
    /// </summary>
    [JsonProperty("client_secret")]
    public string ClientSecret { get; set; }

    /// <summary>
    /// The refresh token value to be used to issue a new access token
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets the OAuth scope being requested by the client application
    /// </summary>
    [JsonProperty("scope")]
    public string Scope => "tracking shipments";

    /// <summary>
    /// Gets the request path
    /// </summary>
    public override string Path => "oauth2/v3/token";

    /// <summary>
    /// Gets the request method
    /// </summary>
    public override HttpMethod Method => HttpMethod.Post;
}