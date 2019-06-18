using System;
using System.Globalization;
using System.Xml.Linq;
using Nop.Plugin.Shipping.USPS.Domain.Extensions;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    /// <summary>
    /// Tracking Detail Information.
    /// </summary>
    public class TrackDetail
    {
        #region Ctor

        public TrackDetail(XElement trackDetailElement)
        {
            if (trackDetailElement is null)
                throw new ArgumentNullException(nameof(trackDetailElement));

            Event = trackDetailElement.GetValueOfXMLElement("Event");
            City = trackDetailElement.GetValueOfXMLElement("EventCity");
            State = trackDetailElement.GetValueOfXMLElement("EventState");
            ZIPCode = trackDetailElement.GetValueOfXMLElement("EventZIPCode");
            Country = trackDetailElement.GetValueOfXMLElement("EventCountry");
            FirmName = trackDetailElement.GetValueOfXMLElement("FirmName");
            Name = trackDetailElement.GetValueOfXMLElement("Name");

            var eventDate = trackDetailElement.GetValueOfXMLElement("EventDate");

            if (!string.IsNullOrEmpty(eventDate) && DateTime.TryParseExact(eventDate, "MMM dd, yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
            {
                Date = date;
            }

        }

        #endregion

        #region Properties

        /// <summary>
        /// The event type (e.g., Enroute).
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        ///The city where the event occurred.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The date and time of the event
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The state where the event occurred.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The ZIP Code of the event.
        /// </summary>
        public string ZIPCode { get; set; }

        /// <summary>
        /// The country where the event occurred.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The company name if delivered to a company.
        /// </summary>
        public string FirmName { get; set; }

        /// <summary>
        /// The name of the persons signing for delivery (if available).
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
