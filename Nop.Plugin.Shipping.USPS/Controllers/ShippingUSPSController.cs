using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.USPS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.USPS.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class ShippingUSPSController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly USPSSettings _uspsSettings;

    #endregion

    #region Ctor

    public ShippingUSPSController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        USPSSettings uspsSettings)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _uspsSettings = uspsSettings;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Orders.SHIPMENTS_VIEW)]
    public async Task<IActionResult> Configure()
    {
        var model = new USPSShippingModel
        {
            UseSandbox = _uspsSettings.UseSandbox,
            TrackingEnabled = _uspsSettings.TrackingEnabled,
            ConsumerKey = _uspsSettings.ConsumerKey,
            ConsumerSecret = _uspsSettings.ConsumerSecret,
            AdditionalHandlingCharge = _uspsSettings.AdditionalHandlingCharge,
            SelectedCarrierDomesticServices = _uspsSettings.CarrierServiceOfferedDomestic,
            SelectedCarrierInternationalServices = _uspsSettings.CarrierServiceOfferedInternational,
            SelectedProcessingCategory = _uspsSettings.ProcessingCategoriesOffered
        };

        model.AvailableDomesticServices.AddRange(USPSShippingDefaults.DomesticMailClasses.Select(x => new SelectListItem(x.Value, x.Key)));
        model.AvailableInternationalServices.AddRange(USPSShippingDefaults.InternationalMailClasses.Select(x => new SelectListItem(x.Value, x.Key)));
        model.AvailableProcessingCategory.AddRange(USPSShippingDefaults.ProcessingCategory.Select(x => new SelectListItem(x.Value, x.Key)));

        return View("~/Plugins/Shipping.USPS/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.SHIPMENTS_CREATE_EDIT_DELETE)]
    public async Task<IActionResult> Configure(USPSShippingModel model, IFormCollection form)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //save settings
        _uspsSettings.UseSandbox = model.UseSandbox;
        _uspsSettings.TrackingEnabled = model.TrackingEnabled;
        _uspsSettings.ConsumerKey = model.ConsumerKey;
        _uspsSettings.ConsumerSecret = model.ConsumerSecret;
        _uspsSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
        _uspsSettings.CarrierServiceOfferedDomestic = model.SelectedCarrierDomesticServices.ToList();
        _uspsSettings.CarrierServiceOfferedInternational = model.SelectedCarrierInternationalServices.ToList();
        _uspsSettings.ProcessingCategoriesOffered = model.SelectedProcessingCategory.ToList();

        await _settingService.SaveSettingAsync(_uspsSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction(nameof(Configure));
    }

    #endregion
}
