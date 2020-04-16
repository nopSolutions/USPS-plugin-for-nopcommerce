using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.USPS.Domain;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.USPS.Services
{
    public class USPSService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IMeasureService _measureService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShippingService _shippingService;
        private readonly IWorkContext _workContext;
        private readonly USPSHttpClient _uspsHttpClient;
        private readonly USPSSettings _uspsSettings;

        #endregion

        #region Ctor

        public USPSService(ILogger logger,
            IMeasureService measureService,
            IShoppingCartService shoppingCartService,
            IShippingService shippingService,
            IWorkContext workContext,
            USPSHttpClient uspsHttpClient,
            USPSSettings uspsSettings)
        {
            _logger = logger;
            _measureService = measureService;
            _shoppingCartService = shoppingCartService;
            _shippingService = shippingService;
            _workContext = workContext;
            _uspsHttpClient = uspsHttpClient;
            _uspsSettings = uspsSettings;
        }

        #endregion

        #region Utilities

        private string CreateRequest(string username, string password, bool isDomestic, GetShippingOptionRequest getShippingOptionRequest)
        {
            var (width, length, height) = GetDimensions(getShippingOptionRequest.Items);
            var weight = GetWeight(getShippingOptionRequest);

            var zipPostalCodeFrom = getShippingOptionRequest.ZipPostalCodeFrom;
            var zipPostalCodeTo = getShippingOptionRequest.ShippingAddress.ZipPostalCode;

            //valid values for testing.
            //Zip to = "20008"; Zip from ="10022"; weight = 2;

            var pounds = Convert.ToInt32(weight / 16);
            var ounces = Convert.ToInt32(weight - (pounds * 16.0M));
            var girth = height + height + width + width;
            //Get shopping cart sub-total.  V2 International rates require the package value to be declared.
            var subTotal = decimal.Zero;
            foreach (var packageItem in getShippingOptionRequest.Items)
            {
                //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
                subTotal += _shoppingCartService.GetSubTotal(packageItem.ShoppingCartItem);
            }

            var rootElementName = isDomestic ? "RateV4Request" : "IntlRateV2Request";

            var rootRequestElement = new XElement(rootElementName,
                new XAttribute("USERID", username),
                new XAttribute("PASSWORD", password),
                new XElement("Revision", 2));

            if (isDomestic)
            {
                #region domestic request

                var xmlStrings = new USPSStrings(); // Create new instance with string array

                if ((!IsPackageTooHeavy(pounds)) && (!IsPackageTooLarge(length, height, width)))
                {
                    var packageSize = GetPackageSize(length, height, width);
                    // RJH get all XML strings not commented out for USPSStrings. 
                    // RJH V3 USPS Service must be Express, Express SH, Express Commercial, Express SH Commercial, First Class, Priority, Priority Commercial, Parcel, Library, BPM, Media, ALL or ONLINE;
                    // AC - Updated to V4 API and made minor improvements to allow First Class Packages (package only - not envelopes).

                    foreach (var element in xmlStrings.Elements) // Loop over elements with property
                    {
                        if ((element == "First Class") && (weight >= 14))
                        {
                            // AC - At the time of coding there aren't any First Class shipping options for packages over 13 ounces. 
                        }
                        else
                        {
                            var packageElement = new XElement("Package", new XAttribute("ID", 0),
                                new XElement("Service", element),
                                new XElement("ZipOrigination", CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(zipPostalCodeFrom), 5)),
                                new XElement("ZipDestination", CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(zipPostalCodeTo), 5)),
                                new XElement("Pounds", pounds),
                                new XElement("Ounces", ounces),
                                new XElement("Container"),
                                new XElement("Size", packageSize),
                                new XElement("Width", width),
                                new XElement("Length", length),
                                new XElement("Height", height),
                                new XElement("Girth", girth),
                                new XElement("Machinable", false));

                            if (element == "First Class")
                                packageElement.Add(new XElement("FirstClassMailType", "PARCEL"));

                            rootRequestElement.Add(packageElement);
                        }
                    }
                }
                else
                {
                    var totalPackagesDims = 1;
                    var totalPackagesWeights = 1;
                    if (IsPackageTooHeavy(pounds))
                    {
                        totalPackagesWeights = Convert.ToInt32(Math.Ceiling(pounds / USPSShippingDefaults.MAX_PACKAGE_WEIGHT));
                    }
                    if (IsPackageTooLarge(length, height, width))
                    {
                        totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / 108M));
                    }
                    var totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                    if (totalPackages == 0)
                        totalPackages = 1;

                    var pounds2 = Math.Max(pounds / totalPackages, 1);
                    //we don't use ounces
                    var ounces2 = Math.Max(ounces / totalPackages, 1);
                    var height2 = Math.Max(height / totalPackages, 1);
                    var width2 = Math.Max(width / totalPackages, 1);
                    var length2 = Math.Max(length / totalPackages, 1);

                    var packageSize = GetPackageSize(length2, height2, width2);

                    var girth2 = height2 + height2 + width2 + width2;

                    for (var i = 0; i < totalPackages; i++)
                    {
                        foreach (var element in xmlStrings.Elements)
                        {
                            if ((element == "First Class") && (weight >= 14))
                            {
                                // AC - At the time of coding there aren't any First Class shipping options for packages over 13 ounces. 
                            }
                            else
                            {
                                var packageElement = new XElement("Package", new XAttribute("ID", i),
                                    new XElement("Service", element),
                                    new XElement("ZipOrigination", zipPostalCodeFrom),
                                    new XElement("ZipDestination", zipPostalCodeTo),
                                    new XElement("Pounds", pounds2),
                                    new XElement("Ounces", ounces2),
                                    new XElement("Container"),
                                    new XElement("Size", packageSize),
                                    new XElement("Width", width2),
                                    new XElement("Length", length2),
                                    new XElement("Height", height2),
                                    new XElement("Girth", girth2),
                                    new XElement("Machinable", false));

                                if (element == "First Class")
                                    packageElement.Add(new XElement("FirstClassMailType", "PARCEL"));

                                rootRequestElement.Add(packageElement);
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region international request

                //V2 International rates require the package value to be declared.  Max content value for most shipping options is $400 so it is limited here.  
                var intlSubTotal = subTotal > 400 ? 400 : subTotal;

                //little hack here for international requests
                length = 12;
                width = 12;
                height = 12;
                girth = height + height + width + width;

                var mailType = "Package"; //Package, Envelope
                var packageSize = GetPackageSize(length, height, width);

                var countryName = FormatCountryForIntlRequest(getShippingOptionRequest);

                if ((!IsPackageTooHeavy(pounds)) && (!IsPackageTooLarge(length, height, width)))
                {

                    var packageElement = new XElement("Package", new XAttribute("ID", 0),
                                    new XElement("Pounds", pounds),
                                    new XElement("Ounces", ounces),
                                    new XElement("Machinable", false),
                                    new XElement("MailType", mailType),
                                    new XElement("GXG",
                                        new XElement("POBoxFlag", "N"),
                                        new XElement("GiftFlag", "N")),
                                    new XElement("ValueOfContents", intlSubTotal),
                                    new XElement("Country", countryName),
                                    new XElement("Container", "RECTANGULAR"),
                                    new XElement("Size", packageSize),
                                    new XElement("Width", width),
                                    new XElement("Length", length),
                                    new XElement("Height", height),
                                    new XElement("Girth", girth),
                                    new XElement("OriginZip", zipPostalCodeFrom),
                                    new XElement("CommercialFlag", "N"));

                    rootRequestElement.Add(packageElement);
                }
                else
                {
                    var totalPackagesDims = 1;
                    var totalPackagesWeights = 1;
                    if (IsPackageTooHeavy(pounds))
                    {
                        totalPackagesWeights = Convert.ToInt32(Math.Ceiling(pounds / USPSShippingDefaults.MAX_PACKAGE_WEIGHT));
                    }
                    if (IsPackageTooLarge(length, height, width))
                    {
                        totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / 108M));
                    }

                    var totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                    if (totalPackages == 0)
                        totalPackages = 1;

                    var pounds2 = pounds / totalPackages;
                    if (pounds2 < 1)
                        pounds2 = 1;
                    //we don't use ounces
                    var ounces2 = ounces / totalPackages;
                    //int height2 = height / totalPackages;
                    //int width2 = width / totalPackages;
                    //int length2 = length / totalPackages;
                    //if (height2 < 1)
                    //    height2 = 1; // Why assign a 1 if it is assigned below 12? Perhaps this is a mistake.
                    //if (width2 < 1)
                    //    width2 = 1; // Similarly
                    //if (length2 < 1)
                    //    length2 = 1; // Similarly

                    //little hack here for international requests (uncomment the code above when fixed)
                    var length2 = 12;
                    var width2 = 12;
                    var height2 = 12;
                    var packageSize2 = GetPackageSize(length2, height2, width2);
                    var girth2 = height2 + height2 + width2 + width2;

                    for (var i = 0; i < totalPackages; i++)
                    {
                        var packageElement = new XElement("Package", new XAttribute("ID", i),
                                    new XElement("Pounds", pounds2),
                                    new XElement("Ounces", ounces2),
                                    new XElement("Machinable", false),
                                    new XElement("MailType", mailType),
                                    new XElement("GXG",
                                        new XElement("POBoxFlag", "N"),
                                        new XElement("GiftFlag", "N")),
                                    new XElement("ValueOfContents", intlSubTotal),
                                    new XElement("Country", countryName),
                                    new XElement("Container", "RECTANGULAR"),
                                    new XElement("Size", packageSize2),
                                    new XElement("Width", width2),
                                    new XElement("Length", length2),
                                    new XElement("Height", height2),
                                    new XElement("Girth", girth2),
                                    new XElement("OriginZip", zipPostalCodeFrom),
                                    new XElement("CommercialFlag", "N"));

                        rootRequestElement.Add(packageElement);
                    }
                }

                #endregion
            }

            return new XDocument(rootRequestElement).ToString();
        }

        /// <summary>
        /// Create request details to track shipment
        /// </summary>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>String with track request details</returns>
        private string CreateTrackRequest(string trackingNumber)
        {
            //<TrackFieldRequest USERID=\"{}\" PASSWORD=\"{}\">
            //    <TrackID ID=\"{}\" />
            //</TrackFieldRequest>

            var document = new XDocument(
                new XElement("TrackFieldRequest", new XAttribute("USERID", _uspsSettings.Username), new XAttribute("PASSWORD", _uspsSettings.Password),
                    new XElement("TrackID", new XAttribute("ID", trackingNumber)))
            );

            return document.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// USPS country hacks
        /// The USPS wants the NAME of the country for international shipments rather than one of the ISO codes
        /// </summary>
        /// <param name="getShippingOptionRequest">Request</param>
        /// <returns></returns>
        private string FormatCountryForIntlRequest(GetShippingOptionRequest shippingOptionRequest)
        {
            var uspsCountriesWithIsoCode = new Dictionary<string, string>
            {
                ["LBY"] = "Cyjrenaica (Libya)", //Libyan Arab Jamahiriya
                ["LAO"] = "Laos", //Lao People's Democratic Republic
                ["FLK"] = "South Georgia (Falkland Islands)", //Falkland Islands (Malvinas)
                ["IRN"] = "Iran", //Iran (Islamic Republic of)
                ["SJM"] = "Svalbard and Jan Mayen Islands",
                ["SWZ"] = "Swaziland (Eswatini)", //Swaziland
                ["VAT"] = "Vatican City", //Vatican City State (Holy See)
                ["SSD"] = "Sudan", // South Sudan - usps only Sudan
                ["ANT"] = "Netherlands", //Netherlands Antilles
                ["PCN"] = "Pitcairn Island", //Pitcairn
                ["BIH"] = "Bosnia-Herzegovina", //Bosnia and Herzegowina
                ["BVT"] = "Norway", //Bouvet Island
                ["CCK"] = "Cocos Island (Australia)", //Cocos (Keeling) Islands
                ["CIV"] = "Ivory Coast", //Cote D'Ivoire
                ["RUS"] = "Russia", //Russian Federation
                ["KOR"] = "South Korea", //Korea
                ["PRK"] = "North Korea" //Korea, Democratic People's Republic of
            };

            return uspsCountriesWithIsoCode.TryGetValue(shippingOptionRequest.CountryFrom.ThreeLetterIsoCode, out var countryName) ?
                countryName : shippingOptionRequest.CountryFrom.Name;
        }

        /// <summary>
        /// Get dimensions values of the package
        /// </summary>
        /// <param name="items">Package items</param>
        /// <returns>Dimensions values</returns>
        private (decimal width, decimal length, decimal height) GetDimensions(IList<GetShippingOptionRequest.PackageItem> items, int minRate = 1)
        {
            var measureDimension = _measureService.GetMeasureDimensionBySystemKeyword(USPSShippingDefaults.MEASURE_DIMENSION_SYSTEM_KEYWORD)
                ?? throw new NopException($"USPS shipping service. Could not load \"{USPSShippingDefaults.MEASURE_DIMENSION_SYSTEM_KEYWORD}\" measure dimension");

            decimal convertAndRoundDimension(decimal dimension)
            {
                dimension = _measureService.ConvertFromPrimaryMeasureDimension(dimension, measureDimension);
                dimension = Convert.ToInt32(Math.Ceiling(dimension));
                return Math.Max(dimension, minRate);
            }

            _shippingService.GetDimensions(items, out var width, out var length, out var height, true);
            width = convertAndRoundDimension(width);
            length = convertAndRoundDimension(length);
            height = convertAndRoundDimension(height);

            return (width, length, height);
        }

        /// <summary>
        /// Get weight value of the package
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request</param>
        /// <returns>Weight value</returns>
        private int GetWeight(GetShippingOptionRequest shippingOptionRequest, int minWeight = 1)
        {
            var measureWeight = _measureService.GetMeasureWeightBySystemKeyword(USPSShippingDefaults.MEASURE_WEIGHT_SYSTEM_KEYWORD)
                ?? throw new NopException($"USPS shipping service. Could not load \"{USPSShippingDefaults.MEASURE_WEIGHT_SYSTEM_KEYWORD}\" measure weight");

            var weight = _shippingService.GetTotalWeight(shippingOptionRequest, ignoreFreeShippedItems: true);
            weight = _measureService.ConvertFromPrimaryMeasureWeight(weight, measureWeight);
            weight = Math.Max(Math.Ceiling(weight), minWeight);
            return Convert.ToInt32(weight);
        }

        private USPSPackageSize GetPackageSize(decimal length, decimal height, decimal width)
        {
            //REGULAR: Package dimensions are 12’’ or less;
            //LARGE: Any package dimension is larger than 12’’.
            if (length > 12 || height > 12 || length > width)
                return USPSPackageSize.Large;

            return USPSPackageSize.Regular;

            //int girth = height + height + width + width;
            //int total = girth + length;
            //if (total <= 84)
            //    return USPSPackageSize.Regular;
            //return USPSPackageSize.Large;
        }

        private bool IsPackageTooHeavy(decimal weight)
        {
            return weight > USPSShippingDefaults.MAX_PACKAGE_WEIGHT;
        }

        private bool IsPackageTooLarge(decimal length, decimal height, decimal width)
        {
            var total = TotalPackageSize(length, height, width);
            return total > 130;
        }

        /// <summary>
        /// Gets shipping rates
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request details</param>
        /// <param name="saturdayDelivery">Whether to get rates for Saturday Delivery</param>
        /// <returns>Shipping options; errors if exist</returns>
        private (IList<ShippingOption> shippingOptions, IEnumerable<string> errors) GetShippingOptions(GetShippingOptionRequest shippingOptionRequest)
        {
            var isDomestic = IsDomesticRequest(shippingOptionRequest);
            var requestString = CreateRequest(_uspsSettings.Username, _uspsSettings.Password, isDomestic, shippingOptionRequest);

            try
            {
                //get rate response
                var rateResponse = _uspsHttpClient.GetRatesAsync(requestString, isDomestic).Result;

                return ParseResponse(rateResponse);
            }
            catch (Exception ex)
            {
                var message = $"USPS Service is currently unavailable, try again later. {ex.Message}";
                //log errors
                _logger.Error(message, ex, shippingOptionRequest.Customer);

                return (new List<ShippingOption>(), new[] { message });
            }
        }

        /// <summary>
        /// Is a request domestic
        /// </summary>
        /// <param name="getShippingOptionRequest">Request</param>
        /// <returns>Result</returns>
        private bool IsDomesticRequest(GetShippingOptionRequest getShippingOptionRequest)
        {
            //Origin Country must be USA, Collect USA from list of countries
            var result = true;
            if (getShippingOptionRequest?.CountryFrom != null)
            {
                switch (getShippingOptionRequest.CountryFrom.ThreeLetterIsoCode)
                {
                    case "USA": // United States
                    case "PRI": // Puerto Rico
                    case "UMI": // United States minor outlying islands
                    case "ASM": // American Samoa
                    case "GUM": // Guam
                    case "MHL": // Marshall Islands
                    case "FSM": // Micronesia
                    case "MNP": // Northern Mariana Islands
                    case "PLW": // Palau
                    case "VIR": // Virgin Islands (U.S.)
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }

        private (IList<ShippingOption> shippingOptions, IEnumerable<string> errors) ParseResponse(RateResponse response)
        {
            var shippingOptions = new List<ShippingOption>();

            if (string.IsNullOrEmpty(_uspsSettings.CarrierServicesOfferedDomestic) || string.IsNullOrEmpty(_uspsSettings.CarrierServicesOfferedInternational))
                return (shippingOptions, null);

            if (!response?.Packages?.Any() ?? true)
                return (shippingOptions, null);

            if (response.Packages.Any(x => x.Error != null))
            {
                return (shippingOptions, response.Packages.Select(x => $"Error Desc: {x.Error.Description}. USPS Help Context: {x.Error.HelpContext}."));
            }

            shippingOptions.AddRange(response.Packages
                .SelectMany(x => x.Postage
                    .Where(isPostageOffered)
                    .Select(p => new ShippingOption
                    {
                        Name = p.MailService,
                        Rate = _uspsSettings.AdditionalHandlingCharge + p.Rate
                    })));

            return (shippingOptions, null);

            // false if the service ID is not in the list of services to offer
            bool isPostageOffered(Postage p)
            {
                var carrierServicesOffered = response.IsDomestic ? _uspsSettings.CarrierServicesOfferedDomestic : _uspsSettings.CarrierServicesOfferedInternational;

                //false if the "First-Class Mail Letter" is not in the list of domestic services to offer
                if (response.IsDomestic && !carrierServicesOffered.Contains("[letter]"))
                {
                    var option = p.MailService.ToLowerInvariant();
                    if (option.Contains("letter") || option.Contains("postcard"))
                        return false;
                }

                // Add delimiters [] so that single digit IDs aren't found in multi-digit IDs                                    
                return carrierServicesOffered.Contains($"[{p.Id}]");
            }
        }

        private decimal TotalPackageSize(decimal length, decimal height, decimal width)
        {
            return height * 2 + width * 2 + length;
        }

        private async Task<IList<ShipmentStatusEvent>> TrackAsync(string requestString)
        {
            var response = await _uspsHttpClient.GetTrackEventsAsync(requestString);

            if (response?.TrackDetails?.Any() ?? false)
            {
                return response.TrackDetails
                    .Select(x => new ShipmentStatusEvent
                    {
                        Date = x.Date,
                        EventName = x.Event,
                        Location = x.City,
                        CountryCode = x.Country
                    })
                    .ToList();
            }

            return new List<ShipmentStatusEvent>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shipping rates
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request details</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public virtual GetShippingOptionResponse GetRates(GetShippingOptionRequest shippingOptionRequest)
        {
            var response = new GetShippingOptionResponse();

            var (shippingOptions, error) = GetShippingOptions(shippingOptionRequest);

            if (!error?.Any() ?? true)
            {
                foreach (var shippingOption in shippingOptions)
                    response.ShippingOptions.Add(shippingOption);
            }
            else
            {
                response.Errors = error.ToList();
            }

            return response;
        }

        /// <summary>
        /// Gets all events for a tracking number
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>Shipment events</returns>
        public virtual IEnumerable<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            try
            {
                //create request details
                var requestString = CreateTrackRequest(trackingNumber);

                //get tracking info
                return TrackAsync(requestString).Result;
            }
            catch (Exception exception)
            {
                //log errors
                var message = $"Error while getting UPS shipment tracking info - {trackingNumber}{Environment.NewLine}{exception.Message}";
                _logger.Error(message, exception, _workContext.CurrentCustomer);

                return new List<ShipmentStatusEvent>();
            }
        }

        #endregion
    }
}
