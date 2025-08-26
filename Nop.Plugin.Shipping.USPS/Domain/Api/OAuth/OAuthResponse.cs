using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.OAuth;

/// <summary>
/// Represents authentication details
/// </summary>
public class OAuthResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the refresh token that is exchanged with the authorisation code
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the access token that is exchanged with the authorisation code
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the remaining lifetime of an refresh token in seconds
    /// </summary>
    [JsonProperty("refresh_token_expires_in")]
    public int RefreshTokenExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the remaining lifetime of an access token in seconds
    /// </summary>
    [JsonProperty("expires_in")]
    public int TokenExpiresIn { get; set; }
}
