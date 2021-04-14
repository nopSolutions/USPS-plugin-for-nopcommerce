using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.USPS.Domain;
using Nop.Plugin.Shipping.USPS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.USPS.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class ShippingUSPSController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly USPSSettings _uspsSettings;

        #endregion

        #region Ctor

        public ShippingUSPSController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            USPSSettings uspsSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _uspsSettings = uspsSettings;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new USPSShippingModel
            {
                Url = _uspsSettings.Url,
                Username = _uspsSettings.Username,
                Password = _uspsSettings.Password,
                AdditionalHandlingCharge = _uspsSettings.AdditionalHandlingCharge
            };

            // Load Domestic service names
            var carrierServicesOfferedDomestic = _uspsSettings.CarrierServicesOfferedDomestic;

            foreach (var service in USPSServices.DomesticServices)
                model.AvailableCarrierServicesDomestic.Add(service);

            if (!string.IsNullOrEmpty(carrierServicesOfferedDomestic))
            {
                foreach (var service in USPSServices.DomesticServices)
                {
                    var serviceId = USPSServices.GetServiceIdDomestic(service);
                    if (!string.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        if (carrierServicesOfferedDomestic.Contains($"[{serviceId}]"))
                            model.CarrierServicesOfferedDomestic.Add(service);
                    }
                }
            }

            // Load Internation service names
            var carrierServicesOfferedInternational = _uspsSettings.CarrierServicesOfferedInternational;
            foreach (var service in USPSServices.InternationalServices)
                model.AvailableCarrierServicesInternational.Add(service);

            if (!string.IsNullOrEmpty(carrierServicesOfferedInternational))
                foreach (var service in USPSServices.InternationalServices)
                {
                    var serviceId = USPSServices.GetServiceIdInternational(service);
                    if (!string.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        if (carrierServicesOfferedInternational.Contains($"[{serviceId}]"))
                            model.CarrierServicesOfferedInternational.Add(service);
                    }
                }
            return View("~/Plugins/Shipping.USPS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(USPSShippingModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //save settings
            _uspsSettings.Url = model.Url;
            _uspsSettings.Username = model.Username;
            _uspsSettings.Password = model.Password;
            _uspsSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;

            // Save selected Domestic services
            var carrierServicesOfferedDomestic = new StringBuilder();
            var carrierServicesDomesticSelectedCount = 0;
            if (model.CheckedCarrierServicesDomestic != null)
            {
                foreach (var cs in model.CheckedCarrierServicesDomestic)
                {
                    carrierServicesDomesticSelectedCount++;

                    var serviceId = USPSServices.GetServiceIdDomestic(cs);
                    //unselect any other services if NONE is selected
                    if (!string.IsNullOrEmpty(serviceId) && serviceId.Equals("NONE"))
                    {
                        carrierServicesOfferedDomestic.Clear();
                        carrierServicesOfferedDomestic.AppendFormat("[{0}]:", serviceId);
                        break;
                    }

                    if (!string.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedDomestic.AppendFormat("[{0}]:", serviceId);
                    }
                }
            }
            // Add default options if no services were selected
            if (carrierServicesDomesticSelectedCount == 0)
                _uspsSettings.CarrierServicesOfferedDomestic = "[1]:[3]:[4]:";
            else
                _uspsSettings.CarrierServicesOfferedDomestic = carrierServicesOfferedDomestic.ToString();

            // Save selected International services
            var carrierServicesOfferedInternational = new StringBuilder();
            var carrierServicesInternationalSelectedCount = 0;
            if (model.CheckedCarrierServicesInternational != null)
            {
                foreach (var cs in model.CheckedCarrierServicesInternational)
                {
                    carrierServicesInternationalSelectedCount++;
                    var serviceId = USPSServices.GetServiceIdInternational(cs);
                    // unselect other services if NONE is selected
                    if (!string.IsNullOrEmpty(serviceId) && serviceId.Equals("NONE"))
                    {
                        carrierServicesOfferedInternational.Clear();
                        carrierServicesOfferedInternational.AppendFormat("[{0}]:", serviceId);
                        break;
                    }
                    if (!string.IsNullOrEmpty(serviceId))
                    {
                        // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs
                        carrierServicesOfferedInternational.AppendFormat("[{0}]:", serviceId);
                    }
                }
            }
            // Add default options if no services were selected
            if (carrierServicesInternationalSelectedCount == 0)
                _uspsSettings.CarrierServicesOfferedInternational = "[2]:[15]:[1]:";
            else
                _uspsSettings.CarrierServicesOfferedInternational = carrierServicesOfferedInternational.ToString();

            await _settingService.SaveSettingAsync(_uspsSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
