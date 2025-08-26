using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Shipping.USPS.Domain.Api;

namespace Nop.Plugin.Shipping.USPS.Services;

public class USPSHttpClient
{
    #region Fields

    private readonly HttpClient _httpClient;
    private readonly USPSSettings _uspsSettings;

    #endregion

    #region Ctor

    public USPSHttpClient(HttpClient client, USPSSettings uspsSettings)
    {
        //configure client
        client.Timeout = TimeSpan.FromSeconds(uspsSettings.ClientTimeout ?? 10);
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        _httpClient = client;
        _uspsSettings = uspsSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request services
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request</param>
    /// <returns>The asynchronous task whose result contains response details</returns>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : IApiRequest where TResponse : IApiResponse
    {
        //prepare request parameters
        var requestString = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, MimeTypes.ApplicationJson);

        var baseUrl = new Uri(_uspsSettings.UseSandbox ? USPSShippingDefaults.SandboxApiUrl : USPSShippingDefaults.ApiUrl);
        var requestMessage = new HttpRequestMessage(request.Method, new Uri(baseUrl, request.Path))
        {
            Content = requestString
        };

        //add authorization
        if (request is IAuthorizedRequest authorized)
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {authorized.Token}");

        //execute request and get result
        var httpResponse = await _httpClient.SendAsync(requestMessage);
        var responseString = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode && !string.IsNullOrEmpty(responseString))
        {
            var result = JsonConvert.DeserializeObject<ApiError>(responseString);

            if (result?.Error is ApiError.ErrorSummary errorSummary)
            {
                var message = $"Request error: {errorSummary.Message}";

                if (errorSummary.Errors?.Any() == true)
                    message += errorSummary.Errors.Aggregate($"{Environment.NewLine}Details:{Environment.NewLine}", (f, s) => f + s.Detail + Environment.NewLine);

                throw new NopException(message);
            }
        }

        try
        {
            var result = JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty);
            return result;
        }
        catch
        {
            throw new NopException($"Request error: {responseString}");
        }
    }

    #endregion
}
