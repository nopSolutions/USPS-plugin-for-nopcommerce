using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.USPS.Models;

public record USPSShippingModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.TrackingEnabled")]
    public bool TrackingEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.ConsumerKey")]
    public string ConsumerKey { get; set; }

    [NoTrim]
    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.ConsumerSecret")]
    [DataType(DataType.Password)]
    public string ConsumerSecret { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.AdditionalHandlingCharge")]
    public decimal AdditionalHandlingCharge { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.CarrierServicesDomestic")]
    public IList<string> SelectedCarrierDomesticServices { get; set; } = new List<string>();
    public List<SelectListItem> AvailableDomesticServices { get; set; } = new();

    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.CarrierServicesInternational")]
    public IList<string> SelectedCarrierInternationalServices { get; set; } = new List<string>();
    public List<SelectListItem> AvailableInternationalServices { get; set; } = new();

    [NopResourceDisplayName("Plugins.Shipping.USPS.Fields.ProcessingCategory")]
    public IList<string> SelectedProcessingCategory { get; set; } = new List<string>();
    public List<SelectListItem> AvailableProcessingCategory { get; set; } = new();
}