namespace Nop.Plugin.Shipping.USPS;

public class USPSShippingDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string SystemName => "Shipping.USPS";

    /// <summary>
    /// Package weight limit (pounds)
    /// </summary>
    public static decimal MaxPackageWeight => 70;

    /// <summary>
    /// Used measure dimension system keyword
    /// </summary>
    public static string MeasureDimensionSystemKeyword => "inches";

    /// <summary>
    /// Used measure weight system keyword
    /// </summary>
    public static string MeasureWeightSystemKeyword => "lb";

    /// <summary>
    /// USPS Api url
    /// </summary>
    public static string ApiUrl => "https://apis.usps.com";

    /// <summary>
    /// Gets the testing API URL
    /// </summary>
    public static string SandboxApiUrl => "https://apis-tem.usps.com";

    /// <summary>
    /// Domestic mail services
    /// </summary>
    public static IReadOnlyDictionary<string, string> DomesticMailClasses => new Dictionary<string, string>()
    {
        ["ALL"] = "All services",
        ["PARCEL_SELECT"] = "Parcel Select",
        ["PRIORITY_MAIL_EXPRESS"] = "Priority Mail Express",
        ["PRIORITY_MAIL"] = "Priority Mail",
        ["LIBRARY_MAIL"] = "Library Mail",
        ["MEDIA_MAIL"] = "Media Mail",
        ["BOUND_PRINTED_MATTER"] = "Bound Printed Matter",
        ["USPS_CONNECT_LOCAL"] = "USPS Connect Local",
        ["USPS_CONNECT_MAIL"] = "USPS Connect Mail",
        ["USPS_CONNECT_REGIONAL"] = "USPS Connect Regional",
        ["USPS_GROUND_ADVANTAGE"] = "USPS Ground Advantage",
        ["DOMESTIC_MATTER_FOR_THE_BLIND"] = "Free Matter for the Blind or Handicapped",
    };

    /// <summary>
    /// International mail services
    /// </summary>
    public static IReadOnlyDictionary<string, string> InternationalMailClasses => new Dictionary<string, string>()
    {
        ["ALL"] = "All services",
        ["FIRST-CLASS_PACKAGE_INTERNATIONAL_SERVICE"] = "First-Class Package International Service",
        ["PRIORITY_MAIL_INTERNATIONAL"] = "Priority Mail International",
        ["PRIORITY_MAIL_EXPRESS_INTERNATIONAL"] = "Priority Mail Express International",
        ["GLOBAL_EXPRESS_GUARANTEED"] = "Global Express Guaranteed"
    };

    /// <summary>
    /// Processing categories
    /// </summary>
    public static IReadOnlyDictionary<string, string> ProcessingCategory => new Dictionary<string, string>()
    {
        ["ALL"] = "All categories",
        ["LETTERS"] = "Letters",
        ["CARDS"] = "Cards",
        ["FLATS"] = "Flats",
        ["MACHINABLE"] = "Machinable Parcel",
        ["IRREGULAR"] = "Irregular Parcel",
        ["NONSTANDARD"] = "Nonstandard parcel",
        ["CATALOGS"] = "Catalogs",
        ["OPEN_AND_DISTRIBUTE"] = "Open and Distribute",
        ["RETURNS"] = "Returns",
        ["SOFT_PACK_MACHINABLE"] = "Soft Pack Machinable",
        ["SOFT_PACK_NON_MACHINABLE"] = "Soft Package Non-machinable"
    };
}
