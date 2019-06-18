using System.Xml.Linq;
using Nop.Plugin.Shipping.USPS.Domain.Extensions;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    public class USPSPackageIntl : USPSPackageBase
    {
        #region Ctor

        public USPSPackageIntl(XElement package) : base(package)
        {
            if (Error != null)
                return;

            Pounds = package.GetValueOfXMLElement<int>("Pounds");
            Ounces = package.GetValueOfXMLElement<int>("Ounces");
            Size = package.GetValueOfXMLElement("Size");
            Machinable = package.GetValueOfXMLElement<bool>("Machinable");

            var id = package.GetValueOfXMLAttribute<int>("ID");
            var rate = package.GetValueOfXMLElement<decimal>("Postage");
            var serviceCode = package.GetValueOfXMLElement("SvcDescription");

            Postage.Add(new Postage(id, rate, serviceCode));
        }

        #endregion
    }
}
