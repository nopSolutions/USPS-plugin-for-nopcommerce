using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Rates;

/// <summary>
/// Represents shipping options response
/// </summary>
public class USPSShippingOptionsResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the originating ZIP Code™ for the package.
    /// </summary>
    [JsonProperty("originZIPCode")]
    public string OriginZIPCode { get; set; }

    /// <summary>
    /// Gets or sets the destination ZIP Code™ for the package
    /// </summary>
    [JsonProperty("destinationZIPCode")]
    public string DestinationZIPCode { get; set; }

    /// <summary>
    /// Gets or sets pricing options
    /// </summary>
    [JsonProperty("pricingOptions")]
    public List<PricingOption> PricingOptions { get; set; }

    #region Nested classes

    /// <summary>
    /// Represents pricing option
    /// </summary>
    public class PricingOption
    {
        /// <summary>
        /// Gets or sets shipping options
        /// </summary>
        [JsonProperty("shippingOptions")]
        public List<ShippingOption> ShippingOptions { get; set; }

        /// <summary>
        /// Gets or sets the price type
        /// </summary>
        [JsonProperty("priceType")]
        public string PriceType { get; set; }
    }

    /// <summary>
    /// Represents shipping options
    /// </summary>
    public class ShippingOption
    {
        /// <summary>
        /// Gets or sets the mail class
        /// </summary>
        [JsonProperty("mailClass")]
        public string MailClass { get; set; }

        /// <summary>
        /// Gets or sets rate options
        /// </summary>
        [JsonProperty("rateOptions")]
        public List<RateOption> RateOptions { get; set; }
    }

    /// <summary>
    /// Represents rate option
    /// </summary>
    public class RateOption
    {
        /// <summary>
        /// Gets or sets
        /// </summary>
        [JsonProperty("commitment")]
        public Commitment Commitment { get; set; }

        /// <summary>
        /// Gets or sets the total price, including the <seealso cref="TotalBasePrice"/> and all extra service prices
        /// </summary>
        [JsonProperty("totalPrice")]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the price of the rate, fees, and pound postage
        /// </summary>
        [JsonProperty("totalBasePrice")]
        public decimal TotalBasePrice { get; set; }

        /// <summary>
        /// Gets or sets rates
        /// </summary>
        [JsonProperty("rates")]
        public List<Rate> Rates { get; set; }
    }

    /// <summary>
    /// Represents commitment and the scheduled delivery date of the package
    /// </summary>
    public class Commitment
    {
        /// <summary>
        /// Gets or sets the name such as 1-day, 2-day, 3-day, Military, DPO
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schedule delivery date
        /// </summary>
        [JsonProperty("scheduleDeliveryDate")]
        public string ScheduleDeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ScheduleDeliveryDate"/> is guaranteed
        /// </summary>
        [JsonProperty("guaranteedDelivery")]
        public bool GuaranteedDelivery { get; set; }
    }

    /// <summary>
    /// Represents shipping rate
    /// </summary>
    public class Rate
    {
        /// <summary>
        /// Gets or sets the description of the price
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets effective start date of the rate
        /// </summary>
        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets effective end date of the rate. If blank the rate doesn't have an end date as of yet.
        /// </summary>
        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        /// <summary>
        /// Gets or sets the amount of Postage Required, does not include insurance or other extra service fees
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets zone
        /// <list type="bullet">
        /// <item>For domestic requests: indicates the calculated zone between the provided <see cref="USPSShippingOptionsRequest.OriginZIPCode"/> and <see cref="DomesticShippingOptionsRequest.DestinationZIPCode"/> for a given mailClass, mailingDate, and weight.</item>
        /// <item>For international requests: indicates the price group for a given <see cref="InternationalShippingOptionsRequest.DestinationCountryCode"/>, mailClass, mailingDate, and rateIndicator.</item>
        /// </list>
        /// </summary>
        [JsonProperty("zone")]
        public string Zone { get; set; }

        /// <summary>
        /// Gets or sets the package weight
        /// </summary>
        [JsonProperty("weight")]
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the dimensional weight of the package, if greater than specified in weight, in ounces
        /// </summary>
        [JsonProperty("dimWeight")]
        public decimal DimWeight { get; set; }

        /// <summary>
        /// Gets or sets fees associated to the package
        /// </summary>
        [JsonProperty("fees")]
        public List<object> Fees { get; set; }

        /// <summary>
        /// Gets or sets the type of price applied
        /// <list type="bullet">
        /// <item>Valid Options for Domestic Prices include: 'RETAIL', 'COMMERCIAL', & 'CONTRACT'</item>
        /// <item>Valid Options for International Prices include: 'RETAIL', 'COMMERCIAL', 'COMMERCIAL_BASE', 'COMMERCIAL_PLUS', & 'CONTRACT'</item>
        /// </list>
        /// </summary>
        [JsonProperty("priceType")]
        public string PriceType { get; set; }

        /// <summary>
        /// Gets or sets the mail service requested
        /// </summary>
        [JsonProperty("mailClass")]
        public string MailClass { get; set; }

        /// <summary>
        /// Gets or sets a business friendly name associated to the product that can be displayed to a customer on a shipping portal
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets a business friendly description associated to the product that can be displayed to a customer on a shipping portal
        /// </summary>
        [JsonProperty("productDefinition")]
        public string ProductDefinition { get; set; }

        /// <summary>
        /// Gets or sets a processing category for the provided rate, this value can be used in the labels API to ensure the provided rate is applied
        /// </summary>
        [JsonProperty("processingCategory")]
        public string ProcessingCategory { get; set; }

        /// <summary>
        /// Gets or sets a two-digit rate indicator code for the provided rate, this value can be used in the labels API to ensure the provided rate is applied
        /// </summary>
        [JsonProperty("rateIndicator")]
        public string RateIndicator { get; set; }

        /// <summary>
        /// Gets or sets the destination Entry Facility type for the provided rate, this value can be used in the labels API to ensure the provided rate is applied
        /// </summary>
        [JsonProperty("destinationEntryFacilityType")]
        public string DestinationEntryFacilityType { get; set; }

        /// <summary>
        /// Gets or sets pricing SKU
        /// </summary>
        [JsonProperty("SKU")]
        public string SKU { get; set; }
    }

    #endregion
}