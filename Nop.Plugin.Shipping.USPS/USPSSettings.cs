using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.USPS;

public class USPSSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox environment
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets the client id
    /// </summary>
    public string ConsumerKey { get; set; }

    /// <summary>
    /// Gets or sets the client secret
    /// </summary>
    public string ConsumerSecret { get; set; }

    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the expire date of a refresh token
    /// </summary>
    public DateTime? RefreshTokenExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the expire date of an access token
    /// </summary>
    public DateTime? TokenExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets an amount of the additional handling charge
    /// </summary>
    public decimal AdditionalHandlingCharge { get; set; }

    /// <summary>
    /// Get or sets available domestic carrier services
    /// </summary>
    public List<string> CarrierServiceOfferedDomestic { get; set; } = new();

    /// <summary>
    /// Get or sets available international carrier services
    /// </summary>
    public List<string> CarrierServiceOfferedInternational { get; set; } = new();

    /// <summary>
    /// Get or sets available international processing categories
    /// </summary>
    public List<string> ProcessingCategoriesOffered { get; set; } = new();

    /// <summary>
    /// Gets or sets a period (in seconds) before the request times out.
    /// </summary>
    public int? ClientTimeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tracking are enabled
    /// </summary>
    public bool TrackingEnabled { get; set; }
}