using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Rates;

/// <summary>
/// Package Definitions
/// </summary>
public class PackageDescription
{
    /// <summary>
    /// Gets or sets the package weight
    /// </summary>
    [JsonProperty("weight")]
    public decimal Weight { get; set; }

    /// <summary>
    /// Gets or sets the package length
    /// </summary>
    [JsonProperty("length")]
    public decimal Length { get; set; }

    /// <summary>
    /// Gets or sets the package height
    /// </summary>
    [JsonProperty("height")]
    public decimal Height { get; set; }

    /// <summary>
    /// Gets or sets the package width
    /// </summary>
    [JsonProperty("width")]
    public decimal Width { get; set; }

    /// <summary>
    /// Gets or sets the package girth
    /// </summary>
    [JsonProperty("girth")]
    public decimal? Girth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the package is non-standard
    /// </summary>
    [JsonProperty("hasNonstandardCharacteristics")]
    public bool HasNonstandardCharacteristics { get; set; }

    /// <summary>
    /// Gets or sets the Mail class
    /// </summary>
    [JsonProperty("mailClass")]
    public string MailClass { get; set; }

    /// <summary>
    /// Gets or sets the merchandise value of the package, in US dollars. Used to calculate Insurance Fees if requested
    /// </summary>
    [JsonProperty("packageValue")]
    public int PackageValue { get; set; }

    /// <summary>
    /// Gets or sets the date package will be mailed. The mailing date may be today plus 0 to 30 days in advance
    /// </summary>
    [JsonProperty("mailingDate")]
    public string MailingDate { get; set; }
}
