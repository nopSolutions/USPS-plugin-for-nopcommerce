namespace Nop.Plugin.Shipping.USPS
{
    public class USPSShippingDefaults
    {
        /// <summary>
        /// Package weight limit
        /// </summary>
        public const decimal MAX_PACKAGE_WEIGHT = 70;

        /// <summary>
        /// Used measure weight system keyword
        /// </summary>
        public const string MEASURE_WEIGHT_SYSTEM_KEYWORD = "ounce";

        /// <summary>
        /// Used measure dimension system keyword
        /// </summary>
        public const string MEASURE_DIMENSION_SYSTEM_KEYWORD = "inches";

        /// <summary>
        /// USPS Api url
        /// </summary>
        public const string DEFAULT_URL = "https://production.shippingapis.com/ShippingAPI.dll";
    }
}
