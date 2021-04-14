using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber)
        {
            var result = new List<ShipmentStatusEvent>();

            if (string.IsNullOrEmpty(trackingNumber))
                return result;

            result.AddRange(await _uspsService.GetShipmentEventsAsync(trackingNumber));

            return result;
        }

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>URL of a tracking page.</returns>
        public Task<string> GetUrlAsync(string trackingNumber)
        {
            return Task.FromResult($"https://tools.usps.com/go/TrackConfirmAction?tLabels={trackingNumber}");
        }

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public Task<bool> IsMatchAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return Task.FromResult(false);

            //What is a FedEx tracking number format?
            return Task.FromResult(false);
        }

        #endregion
    }
}
