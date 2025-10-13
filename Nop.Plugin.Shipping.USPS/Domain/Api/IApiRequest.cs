namespace Nop.Plugin.Shipping.USPS.Domain.Api;

/// <summary>
/// Represents request object
/// </summary>
public interface IApiRequest
{
    /// <summary>
    /// Gets the request path
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the request method
    /// </summary>
    public HttpMethod Method { get; }
}
