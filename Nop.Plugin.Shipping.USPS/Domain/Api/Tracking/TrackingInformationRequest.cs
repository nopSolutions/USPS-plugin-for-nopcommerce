using System.Text.Json.Serialization;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Tracking;

/// <summary>
/// Represents tracking request object
/// </summary>
public class TrackingInformationRequest : ApiRequest, IAuthorizedRequest
{
    /// <summary>
    /// Gets or sets the human-readable representation of package barcode data, commonly known as its tracking number
    /// </summary>
    [JsonIgnore]
    public string TrackingNumber { get; set; }

    #region IApiRequest

    public override string Path => $"tracking/v3/tracking/{TrackingNumber}?expand=DETAIL";
    public override HttpMethod Method => HttpMethod.Get;

    #endregion
}
