using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.USPS.Domain.Api.OAuth;
using Nop.Plugin.Shipping.USPS.Domain.Api.Rates;
using Nop.Plugin.Shipping.USPS.Domain.Api.Tracking;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.USPS.Services;

public class USPSService : IShipmentTracker
{
    #region Fields

    private readonly ICountryService _countryService;
    private readonly ILogger _logger;
    private readonly IMeasureService _measureService;
    private readonly ISettingService _settingService;
    private readonly IShippingService _shippingService;
    private readonly IWorkContext _workContext;
    private readonly USPSHttpClient _uspsHttpClient;
    private readonly USPSSettings _uspsSettings;

    #endregion

    #region Ctor

    public USPSService(ICountryService countryService,
        ILogger logger,
        IMeasureService measureService,
        ISettingService settingService,
        IShippingService shippingService,
        IWorkContext workContext,
        USPSHttpClient uspsHttpClient,
        USPSSettings uspsSettings)
    {
        _countryService = countryService;
        _logger = logger;
        _measureService = measureService;
        _settingService = settingService;
        _shippingService = shippingService;
        _workContext = workContext;
        _uspsHttpClient = uspsHttpClient;
        _uspsSettings = uspsSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="checkConfig">Whether to check configuration</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error if exists
    /// </returns>
    private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function,
        bool checkConfig = true)
    {
        try
        {
            //ensure that plugin is configured
            if (checkConfig && !IsConfigured(_uspsSettings))
                throw new NopException($"{USPSShippingDefaults.SystemName} plugin not configured");

            return (await function(), default);
        }
        catch (Exception exception)
        {
            var logMessage = $"{USPSShippingDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return (default, exception.Message);
        }
    }


    private async Task<DomesticShippingOptionsRequest> CreateDomesticRequestAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        var (width, length, height) = await GetDimensionsAsync(getShippingOptionRequest.Items);
        var weight = await GetWeightAsync(getShippingOptionRequest);
        var girth = 2 * height + 2 * width;

        if (IsPackageTooHeavy(weight))
            throw new NopException("Package is too heavy");

        if (_uspsSettings.CarrierServiceOfferedDomestic is null || !_uspsSettings.CarrierServiceOfferedDomestic.Any())
            return null;

        var (token, _) = await GetAccessTokenAsync();

        return new DomesticShippingOptionsRequest
        {
            OriginZIPCode = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(getShippingOptionRequest.ZipPostalCodeFrom), 5),
            DestinationZIPCode = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(getShippingOptionRequest.ShippingAddress.ZipPostalCode), 5),

            PackageDescription = new PackageDescription
            {
                Weight = weight,
                Length = length,
                Width = width,
                Height = height,
                Girth = girth,
                MailClass = "ALL",
                HasNonstandardCharacteristics = false,
                MailingDate = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
            },
            Token = token
        };
    }

    private async Task<InternationalShippingOptionsRequest> CreateInternatinalRequestAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        var (width, length, height) = await GetDimensionsAsync(getShippingOptionRequest.Items);
        var weight = await GetWeightAsync(getShippingOptionRequest);
        var girth = 2 * height + 2 * width;

        if (IsPackageTooHeavy(weight))
            throw new NopException("Package is too heavy");

        if (_uspsSettings.CarrierServiceOfferedInternational is null || !_uspsSettings.CarrierServiceOfferedInternational.Any())
            return null;

        var shippingCountry = await _countryService.GetCountryByAddressAsync(getShippingOptionRequest.ShippingAddress);
        var (token, _) = await GetAccessTokenAsync();

        return new InternationalShippingOptionsRequest
        {
            OriginZIPCode = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(getShippingOptionRequest.ZipPostalCodeFrom), 5),
            DestinationCountryCode = shippingCountry.TwoLetterIsoCode,
            ForeignPostalCode = getShippingOptionRequest.ShippingAddress.ZipPostalCode,
            PackageDescription = new PackageDescription
            {
                Weight = weight,
                Length = length,
                Width = width,
                Height = height,
                Girth = girth,
                MailClass = "ALL",
                HasNonstandardCharacteristics = false,
            },
            Token = token
        };
    }

    /// <summary>
    /// Get dimensions values of the package
    /// </summary>
    /// <param name="items">Package items</param>
    /// <param name="minRate">Minimal rate</param>
    /// <returns>Dimensions values</returns>
    private async Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> items, int minRate = 1)
    {
        var measureDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync(USPSShippingDefaults.MeasureDimensionSystemKeyword)
            ?? throw new NopException($"USPS shipping service. Could not load \"{USPSShippingDefaults.MeasureDimensionSystemKeyword}\" measure dimension");

        async Task<decimal> convertAndRoundDimensionAsync(decimal dimension)
        {
            dimension = await _measureService.ConvertFromPrimaryMeasureDimensionAsync(dimension, measureDimension);
            dimension = Convert.ToInt32(Math.Ceiling(dimension));
            return Math.Max(dimension, minRate);
        }

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items, true);
        width = await convertAndRoundDimensionAsync(width);
        length = await convertAndRoundDimensionAsync(length);
        height = await convertAndRoundDimensionAsync(height);

        return (width, length, height);
    }

    /// <summary>
    /// Get weight value of the package
    /// </summary>
    /// <param name="shippingOptionRequest">Shipping option request</param>
    /// <param name="minWeight">Minimal weight</param>
    /// <returns>Weight value</returns>
    private async Task<int> GetWeightAsync(GetShippingOptionRequest shippingOptionRequest, int minWeight = 1)
    {
        var measureWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync(USPSShippingDefaults.MeasureWeightSystemKeyword)
            ?? throw new NopException($"USPS shipping service. Could not load \"{USPSShippingDefaults.MeasureWeightSystemKeyword}\" measure weight");

        var weight = await _shippingService.GetTotalWeightAsync(shippingOptionRequest, ignoreFreeShippedItems: true);
        weight = await _measureService.ConvertFromPrimaryMeasureWeightAsync(weight, measureWeight);
        weight = Math.Max(Math.Ceiling(weight), minWeight);

        return Convert.ToInt32(weight);
    }

    /// <summary>
    /// Check whether the plugin is IsConfigured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    private bool IsConfigured(USPSSettings settings)
    {
        return !string.IsNullOrEmpty(settings?.ConsumerKey) && !string.IsNullOrEmpty(settings?.ConsumerSecret);
    }

    /// <summary>
    /// Is a request domestic
    /// </summary>
    /// <param name="getShippingOptionRequest">Request</param>
    /// <returns>Result</returns>
    private async Task<bool> IsDomesticRequestAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        var country = await _countryService.GetCountryByAddressAsync(getShippingOptionRequest?.ShippingAddress);

        //Origin Country must be USA, Collect USA from list of countries
        if (country != null)
            return new[]
            {
                "USA", // United States
                "PRI", // Puerto Rico
                "UMI", // United States minor outlying islands
                "ASM", // American Samoa
                "GUM", // Guam
                "MHL", // Marshall Islands
                "FSM", // Micronesia
                "MNP", // Northern Mariana Islands
                "PLW", // Palau
                "VIR", // Virgin Islands (U.S.)
            }.Contains(country.ThreeLetterIsoCode);

        return false;
    }

    private static bool IsPackageTooHeavy(decimal weight)
    {
        return weight > USPSShippingDefaults.MaxPackageWeight;
    }

    /// <summary>
    /// Get access token
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the access token; error message if exists
    /// </returns>
    private async Task<(string Token, string Error)> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_uspsSettings.AccessToken) && _uspsSettings.TokenExpiresIn >= DateTime.Now)
            return (_uspsSettings.AccessToken, string.Empty);

        return await HandleFunctionAsync(async () =>
        {
            var result = await _uspsHttpClient.RequestAsync<OAuthApiRequest, OAuthResponse>(new()
            {
                GrantType = string.IsNullOrEmpty(_uspsSettings.RefreshToken) ? "client_credentials" : "refresh_token",
                ClientId = _uspsSettings.ConsumerKey,
                ClientSecret = _uspsSettings.ConsumerSecret,
                RefreshToken = _uspsSettings.RefreshToken,
            });

            _uspsSettings.AccessToken = result.AccessToken;
            _uspsSettings.TokenExpiresIn = DateTime.Now.AddSeconds(result.TokenExpiresIn);
            _uspsSettings.RefreshToken = result.RefreshToken;
            _uspsSettings.RefreshTokenExpiresIn = DateTime.Now.AddSeconds(result.RefreshTokenExpiresIn);
            await _settingService.SaveSettingAsync(_uspsSettings);

            return result.AccessToken;
        });
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets shipping rates
    /// </summary>
    /// <param name="shippingOptionRequest">Shipping option request details</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the represents a response of getting shipping rate options
    /// </returns>
    public async Task<GetShippingOptionResponse> GetRatesAsync(GetShippingOptionRequest shippingOptionRequest)
    {
        var response = new GetShippingOptionResponse();
        var isDomestic = await IsDomesticRequestAsync(shippingOptionRequest);

        var (shippingOptions, error) = await HandleFunctionAsync(async () =>
        {
            //get rate response
            USPSShippingOptionsRequest request = isDomestic ?
                await CreateDomesticRequestAsync(shippingOptionRequest) : await CreateInternatinalRequestAsync(shippingOptionRequest);

            return request is null ? null : await _uspsHttpClient.RequestAsync<USPSShippingOptionsRequest, USPSShippingOptionsResponse>(request);
        });

        if (!string.IsNullOrEmpty(error))
        {
            response.Errors.Add("USPS Service. Unable to retrieve shipping options");
            return response;
        }

        if (shippingOptions?.PricingOptions is null)
            return response;

        var pricing = shippingOptions.PricingOptions.First();

        if (pricing.ShippingOptions is null)
            return response;

        var mailClasses = isDomestic ? _uspsSettings.CarrierServiceOfferedDomestic : _uspsSettings.CarrierServiceOfferedInternational;
        var rateOptions = pricing.ShippingOptions
            .Where(s => mailClasses.Contains("ALL") || mailClasses.Contains(s.MailClass))
            .SelectMany(x => x.RateOptions)
            .ToList();

        foreach (var option in rateOptions)
        {
            var commitment = option.Commitment;
            var rates = option.Rates
                .Where(r => _uspsSettings.ProcessingCategoriesOffered.Contains("ALL") || _uspsSettings.ProcessingCategoriesOffered.Contains(r.ProcessingCategory))
                .ToList();

            foreach (var rate in rates)
            {
                response.ShippingOptions.Add(new()
                {
                    Name = rate.Description,
                    Rate = _uspsSettings.AdditionalHandlingCharge + rate.Price,
                    TransitDays = commitment is null ? null : (int)Math.Ceiling((Convert.ToDateTime(commitment.ScheduleDeliveryDate) - DateTime.UtcNow).TotalDays)
                });
            }
        }

        return response;
    }

    #region Tracker

    /// <summary>
    /// Get all shipment events
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of shipment events
    /// </returns>
    public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null)
    {
        if (string.IsNullOrEmpty(trackingNumber))
            return null;

        if (!_uspsSettings.TrackingEnabled)
            return null;

        var (token, _) = await GetAccessTokenAsync();
        var request = new TrackingInformationRequest { TrackingNumber = trackingNumber, Token = token };

        var (trackingEvents, error) = await HandleFunctionAsync(async () =>
        {
            //get tracking info
            var response = await _uspsHttpClient.RequestAsync<TrackingInformationRequest, TrackingInformationResponse>(request);

            if (response?.TrackingEvents?.Any() != true)
                return new List<ShipmentStatusEvent>();

            return response.TrackingEvents
                    .Select(x => new ShipmentStatusEvent
                    {
                        Date = x.EventTimestamp,
                        EventName = x.EventType,
                        Location = string.Join(", ", new string[] { x.EventCountry, x.EventCity, x.EventZIP }.Where(s => !string.IsNullOrWhiteSpace(s))),
                        CountryCode = x.EventCountry
                    }).ToList();
        });

        if (!string.IsNullOrEmpty(error))
            return null;

        return trackingEvents;
    }

    /// <summary>
    /// Get URL for a page to show tracking info (third party tracking page)
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL of a tracking page
    /// </returns>
    public Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null)
    {
        return Task.FromResult(string.Empty);
    }

    #endregion

    #endregion
}
