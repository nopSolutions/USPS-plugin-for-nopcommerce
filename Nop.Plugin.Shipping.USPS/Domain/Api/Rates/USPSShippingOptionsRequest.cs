using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Rates;

/// <summary>
/// Represents shipping options request
/// </summary>
public abstract class USPSShippingOptionsRequest : ApiRequest, IAuthorizedRequest
{
    /// <summary>
    /// Gets or sets the originating ZIP code for the package.
    /// </summary>
    [JsonProperty("originZIPCode")]
    public string OriginZIPCode { get; set; }

    /// <summary>
    /// Gets or sets package definitions
    /// </summary>
    [JsonProperty("packageDescription")]
    public PackageDescription PackageDescription { get; set; }

    /// <summary>
    /// Gets or sets price options of the shipping request
    /// </summary>
    [JsonProperty("pricingOptions")]
    public List<PricingOption> PricingOptions => [new()];

    #region ApiRequest

    public override HttpMethod Method => HttpMethod.Post;
    public override string Path => "shipments/v3/options/search";

    #endregion

    #region Nested classes

    /// <summary>
    /// Represents the price options of the shipping request
    /// </summary>
    public class PricingOption
    {
        /// <summary>
        /// Gets or sets the price type of the shipping request
        /// </summary>
        [JsonProperty("priceType")]
        public static string PriceType => "RETAIL";
    }

    #endregion
}
