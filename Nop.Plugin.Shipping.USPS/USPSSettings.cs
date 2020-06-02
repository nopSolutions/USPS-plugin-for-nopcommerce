using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.USPS
{
    public class USPSSettings : ISettings
    {
        /// <summary>
        /// Gets or sets USPS URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets an amount of the additional handling charge
        /// </summary>
        public decimal AdditionalHandlingCharge { get; set; }

        /// <summary>
        /// Get or sets available domestic carrier services
        /// </summary>
        public string CarrierServicesOfferedDomestic { get; set; }

        /// <summary>
        /// Get or sets available international carrier services
        /// </summary>
        public string CarrierServicesOfferedInternational { get; set; }

        /// <summary>
        /// Gets or sets a period (in seconds) before the request times out.
        /// </summary>
        public int? ClientTimeout { get; set; }
    }
}