//------------------------------------------------------------------------------
// Contributor(s): RJH 08/07/2009, mb 10/20/2010, AC 05/16/2011.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Shipping.USPS.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.USPS
{
    /// <summary>
    /// USPS computation method
    /// </summary>
    public class USPSComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly USPSService _uspsService;

        #endregion

        #region Ctor

        public USPSComputationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            USPSService uspsService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _uspsService = uspsService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest is null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            if (!getShippingOptionRequest.Items?.Any() ?? true)
                return new GetShippingOptionResponse { Errors = new[] { "No shipment items" } };

            if (getShippingOptionRequest.ShippingAddress?.CountryId is null)
                return new GetShippingOptionResponse { Errors = new[] { "Shipping address is not set" } };

            return await _uspsService.GetRatesAsync(getShippingOptionRequest);
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains the fixed shipping rate; or null in case there's no fixed shipping rate
        /// </returns>
        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult<decimal?>(null);
        }

        /// <summary>
        /// Get associated shipment tracker
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment tracker
        /// </returns>
        public Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            return Task.FromResult<IShipmentTracker>(null);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ShippingUSPS/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new USPSSettings
            {
                Url = USPSShippingDefaults.DEFAULT_URL,
                Username = "123",
                Password = "456",
                ClientTimeout = 10,
                AdditionalHandlingCharge = 0,
                CarrierServicesOfferedDomestic = "",
                CarrierServicesOfferedInternational = ""
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Shipping.USPS.Fields.Url"] = "URL",
                ["Plugins.Shipping.USPS.Fields.Url.Hint"] = "Specify USPS URL.",
                ["Plugins.Shipping.USPS.Fields.Username"] = "Username",
                ["Plugins.Shipping.USPS.Fields.Username.Hint"] = "Specify USPS username.",
                ["Plugins.Shipping.USPS.Fields.Password"] = "Password",
                ["Plugins.Shipping.USPS.Fields.Password.Hint"] = "Specify USPS password.",
                ["Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge"] = "Additional handling charge",
                ["Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge.Hint"] = "Enter additional handling fee to charge your customers.",
                ["Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic"] = "Domestic Carrier Services",
                ["Plugins.Shipping.USPS.Fields.AvailableCarrierServicesDomestic.Hint"] = "Select the services you want to offer to customers.",
                ["Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational"] = "International Carrier Services",
                ["Plugins.Shipping.USPS.Fields.AvailableCarrierServicesInternational.Hint"] = "Select the services you want to offer to customers."
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<USPSSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.USPS");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker => new USPSShipmentTracker(_uspsService);

        #endregion
    }
}