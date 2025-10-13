using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.USPS.Domain.Api.Tracking;

/// <summary>
/// Represents tracking information response
/// </summary>
public class TrackingInformationResponse : ApiResponse
{
    /// <summary>
    /// Gets or sets the destination city
    /// </summary>
    [JsonProperty("destinationCity")]
    public string DestinationCity { get; set; }

    /// <summary>
    /// Gets or sets the destination state code (2 characters)
    /// <code>AA|AE|AL|AK|AP|AS|AZ|AR|CA|CO|CT|DE|DC|FM|FL|GA|GU|HI|ID|IL|IN|IA|KS|KY|LA|ME|MH|MD|MA|MI|MN|MS|MO|MP|MT|NE|NV|NH|NJ|NM|NY|NC|ND|OH|OK|OR|PW|PA|PR|RI|SC|SD|TN|TX|UT|VT|VI|VA|WA|WV|WI|WY</code>
    /// </summary>
    [JsonProperty("destinationState")]
    public string DestinationState { get; set; }

    /// <summary>
    /// Gets or sets the destination ZIP Code™
    /// </summary>
    [JsonProperty("destinationZIP")]
    public string DestinationZIP { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the USPS® Tracking by Email service is enabled
    /// </summary>
    [JsonProperty("emailEnabled")]
    public string EmailEnabled { get; set; }

    /// <summary>
    /// Gets or sets Kahala Posts Group (KPG) member indicator
    /// </summary>
    [JsonProperty("kahalaIndicator")]
    public string KahalaIndicator { get; set; }

    /// <summary>
    /// Gets or sets the mail class
    /// </summary>
    [JsonProperty("mailClass")]
    public string MailClass { get; set; }

    /// <summary>
    /// Gets or sets the mail type
    /// </summary>
    [JsonProperty("mailType")]
    public string MailType { get; set; }

    /// <summary>
    /// Gets or sets the mail item origin city
    /// </summary>
    [JsonProperty("originCity")]
    public string OriginCity { get; set; }

    /// <summary>
    /// Gets or sets the mail item origin state or province (<= 3 characters)
    /// </summary>
    [JsonProperty("originState")]
    public string OriginState { get; set; }

    /// <summary>
    /// Gets or sets the mail item origin ZIP Code™
    /// </summary>
    [JsonProperty("originZIP")]
    public string OriginZIP { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Proof of Delivery service is enabled
    /// </summary>
    [JsonProperty("proofOfDeliveryEnabled")]
    public string ProofOfDeliveryEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Restore tracking information service is enabled
    /// </summary>
    [JsonProperty("restoreEnabled")]
    public string RestoreEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Return Receipt After Mailing service is enabled
    /// </summary>
    [JsonProperty("RRAMEnabled")]
    public string RRAMEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Receipt Electronic service is enabled
    /// </summary>
    [JsonProperty("RREEnabled")]
    public string RREEnabled { get; set; }

    /// <summary>
    /// Gets or sets additional services purchased
    /// </summary>
    [JsonProperty("services")]
    public List<string> Services { get; set; }

    /// <summary>
    /// Gets or sets the service Type Code of the mail piece
    /// </summary>
    [JsonProperty("serviceTypeCode")]
    public string ServiceTypeCode { get; set; }

    /// <summary>
    /// Gets or sets the item status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the item status category
    /// </summary>
    [JsonProperty("statusCategory")]
    public string StatusCategory { get; set; }

    /// <summary>
    /// Gets or sets the status summary
    /// </summary>
    [JsonProperty("statusSummary")]
    public string StatusSummary { get; set; }

    /// <summary>
    /// Gets or sets the detailed tracking event information for the requested tracking number in reverse chronological order 
    /// </summary>
    [JsonProperty("trackingEvents")]
    public List<TrackingEvent> TrackingEvents { get; set; }

    /// <summary>
    /// Gets or sets the human-readble representation of package barcode data, commonly known as its tracking number
    /// </summary>
    [JsonProperty("trackingNumber")]
    public string TrackingNumber { get; set; }

    #region Nested classes

    /// <summary>
    /// Represents the detailed tracking event information
    /// </summary>
    public class TrackingEvent
    {
        /// <summary>
        /// Gets or sets the type of event
        /// </summary>
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the date of the event
        /// </summary>
        [JsonProperty("eventTimestamp")]
        public DateTime EventTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the country where the event occurred
        /// </summary>
        [JsonProperty("eventCountry")]
        public string EventCountry { get; set; }

        /// <summary>
        /// Gets or sets the city where the event occurred
        /// </summary>
        [JsonProperty("eventCity")]
        public string EventCity { get; set; }

        /// <summary>
        /// Gets or sets the state where the event occurred
        /// </summary>
        [JsonProperty("eventState")]
        public string EventState { get; set; }

        /// <summary>
        /// Gets or sets the ZIP Code™ of the event
        /// </summary>
        [JsonProperty("eventZIP")]
        public string EventZIP { get; set; }

        /// <summary>
        /// Gets or sets the company name if delivered to a company
        /// </summary>
        [JsonProperty("firm")]
        public string Firm { get; set; }

        /// <summary>
        /// Gets or sets the name of the persons signing for delivery (if available)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person signing is an Authorized Agent
        /// </summary>
        [JsonProperty("authorizedAgent")]
        public string AuthorizedAgent { get; set; }

        /// <summary>
        /// Gets or sets refer to the lookup table for all the possible values
        /// </summary>
        [JsonProperty("eventCode")]
        public string EventCode { get; set; }

        /// <summary>
        /// Gets or sets additional property
        /// </summary>
        [JsonProperty("additionalProp")]
        public string AdditionalProp { get; set; }
    }

    #endregion
}
