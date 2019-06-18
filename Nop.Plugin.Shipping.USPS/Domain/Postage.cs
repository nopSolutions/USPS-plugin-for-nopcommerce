using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Nop.Plugin.Shipping.USPS.Domain.Extensions;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    /// <summary>
    /// Represents nested postal rate and service description.  
    /// </summary>
    public class Postage
    {
        #region Ctor

        public Postage(XElement postage)
        {
            if (postage is null)
                throw new ArgumentNullException(nameof(postage));

            Id = postage.GetValueOfXMLAttribute<int>("CLASSID");

            Rate = postage.GetValueOfXMLElement<decimal>("Rate");
            MailService = PrepareServiceCode(postage.GetValueOfXMLElement("MailService"));
        }

        public Postage(int id, decimal rate, string serviceCode)
        {
            Id = id;
            Rate = rate;
            MailService = PrepareServiceCode(serviceCode);
        }

        #endregion

        /// <summary>
        /// Gets or sets a mail class identifier for the postage returned
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a service type name
        /// </summary>
        public string MailService { get; set; }

        /// <summary>
        /// Gets or sets a retail rate
        /// </summary>
        public decimal Rate { get; set; }

        #region Utilities

        private string PrepareServiceCode(string serviceCode)
        {
            //USPS issue fixed
            var reg = (char)174; // registered sign "\u00AE"
            var tm = "\u2122"; // trademark sign
            serviceCode = serviceCode.Replace("&lt;sup&gt;&amp;reg;&lt;/sup&gt;", reg.ToString());
            serviceCode = serviceCode.Replace("&lt;sup&gt;&#174;&lt;/sup&gt;", reg.ToString());
            serviceCode = serviceCode.Replace("&lt;sup&gt;&amp;trade;&lt;/sup&gt;", tm);
            serviceCode = serviceCode.Replace("&lt;sup&gt;&#8482;&lt;/sup&gt;", tm);

            return Regex.Replace(serviceCode, "^(USPS\\s*)|^()", "USPS ", RegexOptions.IgnoreCase);
        }

        #endregion

    }
}
