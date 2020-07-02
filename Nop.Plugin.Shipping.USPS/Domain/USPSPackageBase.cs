using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Nop.Plugin.Shipping.USPS.Domain.Extensions;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    /// <summary>
    /// Postal rate
    /// </summary>
    public abstract class USPSPackageBase
    {
        #region Ctor

        public USPSPackageBase(XElement package)
        {
            if (package is null)
                throw new ArgumentNullException(nameof(package));

            Id = package.Attribute("ID")?.Value ?? string.Empty;

            var elem = package.Element("Error") ?? (package.Name == "Error" ? package : null);

            if (elem is XElement error)
            {
                Error = new ResponseError
                {
                    Number = error.GetValueOfXMLElement("Number"),
                    Description = error.GetValueOfXMLElement("Description"),
                    Source = error.GetValueOfXMLElement("Source"),
                    HelpContext = error.GetValueOfXMLElement("HelpContext"),
                    HelpFile = error.GetValueOfXMLElement("HelpFile")
                };
            }


        }

        #endregion

        /// <summary>
        /// Corresponds to ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Package Weight (Pounds)
        /// </summary>
        public int Pounds { get; set; }

        /// <summary>
        /// Package Weight (Ounces)
        /// </summary>
        public int Ounces { get; set; }

        /// <summary>
        /// Package Size
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Machinable (appears where applicable: RateV4Request[Service='ALL' or Service='FIRST CLASS' or Service=’ Retail Ground’])  
        /// </summary>
        public bool Machinable { get; set; }

        /// <summary>
        /// Postage contains a nested postal rate and service description.  
        /// </summary>
        public IList<Postage> Postage { get; } = new List<Postage>();

        public ResponseError Error { get; set; }
    }
}
