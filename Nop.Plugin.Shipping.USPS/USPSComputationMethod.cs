//------------------------------------------------------------------------------
// Contributor(s): RJH 08/07/2009, mb 10/20/2010, AC 05/16/2011.
//------------------------------------------------------------------------------

using Nop.Core;
using Nop.Plugin.Shipping.USPS.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.USPS;

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
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        if (!getShippingOptionRequest.Items?.Any() ?? true)
            return new GetShippingOptionResponse { Errors = ["No shipment items"] };

        var shippingAddress = getShippingOptionRequest.ShippingAddress;
        if (shippingAddress is null || shippingAddress.CountryId is null || string.IsNullOrEmpty(shippingAddress.ZipPostalCode))
            return new GetShippingOptionResponse { Errors = ["Shipping address is not set"] };

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
        return Task.FromResult<IShipmentTracker>(_uspsService);
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
            ClientTimeout = 10,
            AdditionalHandlingCharge = 0,
            CarrierServiceOfferedDomestic = ["ALL"],
            CarrierServiceOfferedInternational = ["ALL"],
            ProcessingCategoriesOffered = ["ALL"],
            TrackingEnabled = false
        };
        await _settingService.SaveSettingAsync(settings);

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Shipping.USPS.Fields.ConsumerKey"] = "Consumer key",
            ["Plugins.Shipping.USPS.Fields.ConsumerKey.Hint"] = "Specify the consumer key.",
            ["Plugins.Shipping.USPS.Fields.ConsumerSecret"] = "Consumer secret",
            ["Plugins.Shipping.USPS.Fields.ConsumerSecret.Hint"] = "Specify the consumer secret.",
            ["Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge"] = "Additional handling charge",
            ["Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge.Hint"] = "Enter additional handling fee to charge your customers.",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesDomestic"] = "Domestic Carrier Services",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesDomestic.Hint"] = "Select the services you want to offer to customers.",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesInternational"] = "International Carrier Services",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesInternational.Hint"] = "Select the services you want to offer to customers.",
            ["Plugins.Shipping.USPS.Fields.ProcessingCategory"] = "Processing categories",
            ["Plugins.Shipping.USPS.Fields.ProcessingCategory.Hint"] = "Select processing categories for the available rates.",
            ["Plugins.Shipping.USPS.Fields.UseSandbox"] = "Use sandbox",
            ["Plugins.Shipping.USPS.Fields.UseSandbox.Hint"] = "Check to use sandbox (testing environment).",
            ["Plugins.Shipping.USPS.Fields.TrackingEnabled"] = "Tracking",
            ["Plugins.Shipping.USPS.Fields.TrackingEnabled.Hint"] = "Check to enable the tracking API. The default scopes include OAuth, Addresses, Service Standards, Locations, Service Standards Files, International Pricing, Domestic Pricing, and Shipping Options, each with a quota of 60 calls per hour. To get started with the tracking API, please contact the USPS team by submitting a USPS API service request.",
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
}