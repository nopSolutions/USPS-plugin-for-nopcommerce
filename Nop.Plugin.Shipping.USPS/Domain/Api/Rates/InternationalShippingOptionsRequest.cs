using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Rates;

/// <summary>
/// Represents domestic shipping options request
/// </summary>
public class InternationalShippingOptionsRequest : USPSShippingOptionsRequest
{
    /// <summary>
    /// A 2-digit country code is required for Country of destination.
    /// </summary>
    [JsonProperty("destinationCountryCode")]
    public string DestinationCountryCode { get; set; }

    /// <summary>
    /// The foreign ZIP Code™ for the package.
    /// </summary>
    [JsonProperty("foreignPostalCode")]
    public string ForeignPostalCode { get; set; }
}
