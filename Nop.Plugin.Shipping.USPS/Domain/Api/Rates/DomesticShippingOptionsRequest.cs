using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Rates;

/// <summary>
/// Represents domestic shipping options request
/// </summary>
public class DomesticShippingOptionsRequest : USPSShippingOptionsRequest, IAuthorizedRequest
{
    /// <summary>
    /// The destination ZIP code for the package.
    /// </summary>
    [JsonProperty("destinationZIPCode")]
    public string DestinationZIPCode { get; set; }
}
