using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    /// <summary>
    /// Represents response of Rate Calculator APIs (see https://www.usps.com/business/web-tools-apis/rate-calculator-api.htm)
    /// </summary>
    public class RateResponse
    {
        #region Ctor

        public RateResponse()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the information is domestic package
        /// </summary>
        public bool IsDomestic { get; set; }

        /// <summary>
        /// Gets or sets the collection of packages
        /// </summary>
        public IList<USPSPackageBase> Packages { get; set; } = new List<USPSPackageBase>();

        #endregion

        #region Methods

        /// <summary>
        /// Load rates from the passed stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="isDomestic">Value indicating whether the information is domestic package</param>
        /// <returns>The asynchronous task whose result contains the RSS feed</returns>
        public static async Task<RateResponse> LoadAsync(Stream stream, bool isDomestic)
        {
            var response = new RateResponse
            {
                IsDomestic = isDomestic
            };

            try
            {
                var document = await XDocument.LoadAsync(stream, LoadOptions.None, default);

                if (document?.Root is null)
                    return response;

                var error = document?.Root.Name == "Error" ? document?.Root : null;
                if (error != null)
                {
                    response.Packages.Add(new USPSPackageIntl(error));
                }

                foreach (var package in document?.Root.Elements("Package"))
                {
                    if (isDomestic)
                    {
                        //Domestic response rates
                        response.Packages.Add(new USPSPackage(package));
                        continue;
                    }

                    //Has errors?
                    if (!package.Elements("Service")?.Any() ?? true)
                    {
                        response.Packages.Add(new USPSPackageIntl(package));
                        continue;
                    }

                    //International response rates
                    foreach (var packageIntl in package.Elements("Service"))
                    {
                        response.Packages.Add(new USPSPackageIntl(packageIntl));
                    }
                }

                return response;
            }
            catch
            {
                return response;
            }
        }

        #endregion
    }
}
