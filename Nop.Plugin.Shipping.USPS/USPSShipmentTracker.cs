using System.Collections.Generic;
using Nop.Plugin.Shipping.USPS.Services;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.USPS
{
    public class USPSShipmentTracker : IShipmentTracker
    {
        #region Fields

        private readonly USPSService _uspsService;

        #endregion

        #region Ctor

        public USPSShipmentTracker(USPSService uspsService)
        {
            _uspsService = uspsService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            var result = new List<ShipmentStatusEvent>();

            if (string.IsNullOrEmpty(trackingNumber))
                return result;

            result.AddRange(_uspsService.GetShipmentEvents(trackingNumber));

            return result;
        }

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>URL of a tracking page.</returns>
        public string GetUrl(string trackingNumber)
        {
            return $"https://tools.usps.com/go/TrackConfirmAction?tLabels={trackingNumber}";
        }

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public bool IsMatch(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return false;

            //What is a FedEx tracking number format?
            return false;
        }

        #endregion
    }
}
