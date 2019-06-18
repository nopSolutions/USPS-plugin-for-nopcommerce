using System.Xml.Linq;
using Nop.Plugin.Shipping.USPS.Domain.Extensions;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    public class USPSPackage : USPSPackageBase
    {
        #region Ctor

        public USPSPackage(XElement package) : base(package)
        {
            if (Error != null)
                return;

            Pounds = package.GetValueOfXMLElement<int>("Pounds");
            Ounces = package.GetValueOfXMLElement<int>("Ounces");
            Size = package.GetValueOfXMLElement("Size");
            Machinable = package.GetValueOfXMLElement<bool>("Machinable");
            ZipDestination = package.GetValueOfXMLElement("ZipDestination");
            ZipOrigination = package.GetValueOfXMLElement("ZipOrigination");
            Zone = package.GetValueOfXMLElement<int>("Zone");

            foreach (var item in package.Elements("Postage"))
            {
                Postage.Add(new Postage(item));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Origination ZIP Code
        /// </summary>
        public string ZipOrigination { get; set; }

        /// <summary>
        /// Destination ZIP Code  
        /// </summary>
        public string ZipDestination { get; set; }

        /// <summary>
        /// Returned if zone for the postage service is different than the zone for package tag.
        /// Postal Zone indicates the number of postal rate zones between the origin and destination ZIP codes.
        /// </summary>
        public int Zone { get; set; }

        #endregion
    }
}
