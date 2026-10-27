using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Shipping.USPS.Migrations;


[NopMigration("2025-10-21 00:00:00", "USPS. Update New API", MigrationProcessType.Update)]
public class MigrationNewApi : MigrationBase
{
    #region Methods

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        #region Settings

        this.SetSettingIfNotExists<USPSSettings, List<string>>(settings => settings.CarrierServiceOfferedDomestic, ["ALL"]);
        this.SetSettingIfNotExists<USPSSettings, List<string>>(settings => settings.CarrierServiceOfferedInternational, ["ALL"]);
        this.SetSettingIfNotExists<USPSSettings, List<string>>(settings => settings.ProcessingCategoriesOffered, ["ALL"]);
        this.SetSettingIfNotExists<USPSSettings, bool>(settings => settings.TrackingEnabled, false);

        #endregion

        #region Locales

        this.DeleteLocaleResources(new List<string>
        {
            "Plugins.Shipping.USPS.Fields.Url",
            "Plugins.Shipping.USPS.Fields.Url.Hint"
        });

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Shipping.USPS.Fields.ConsumerKey"] = "Consumer key",
            ["Plugins.Shipping.USPS.Fields.ConsumerKey.Hint"] = "Specify the consumer key.",
            ["Plugins.Shipping.USPS.Fields.ConsumerSecret"] = "Consumer secret",
            ["Plugins.Shipping.USPS.Fields.ConsumerSecret.Hint"] = "Specify the consumer secret.",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesDomestic"] = "Classes of Domestic Mail",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesDomestic.Hint"] = "Select the services you want to offer to customers.",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesInternational"] = "Classes of International Mail",
            ["Plugins.Shipping.USPS.Fields.CarrierServicesInternational.Hint"] = "Select the services you want to offer to customers.",
            ["Plugins.Shipping.USPS.Fields.ProcessingCategory"] = "Processing categories",
            ["Plugins.Shipping.USPS.Fields.ProcessingCategory.Hint"] = "Select processing categories for the available rates.",
            ["Plugins.Shipping.USPS.Fields.UseSandbox"] = "Use sandbox",
            ["Plugins.Shipping.USPS.Fields.UseSandbox.Hint"] = "Check to use sandbox (testing environment).",
            ["Plugins.Shipping.USPS.Fields.TrackingEnabled"] = "Tracking",
            ["Plugins.Shipping.USPS.Fields.TrackingEnabled.Hint"] = "Check to enable the tracking API. The default scopes include OAuth, Addresses, Service Standards, Locations, Service Standards Files, International Pricing, Domestic Pricing, and Shipping Options, each with a quota of 60 calls per hour. To get started with the tracking API, please contact the USPS team by submitting a USPS API service request.",
        });

        #endregion

    }
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }

    #endregion
}
