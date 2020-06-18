using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    public class TrackInfo
    {
        #region Ctor

        public TrackInfo(string trackId, TrackDetail summary)
        {
            TrackId = trackId;
            TrackSummary = summary;
            TrackDetails = new List<TrackDetail>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Package Tracking ID number. 
        /// </summary>
        public string TrackId { get; set; }

        /// <summary>
        /// Tracking Summary Information.
        /// </summary>
        public TrackDetail TrackSummary { get; set; }

        /// <summary>
        /// Tracking Detail Information.
        /// </summary>
        public IList<TrackDetail> TrackDetails { get; set; }

        #endregion

        #region Methods

        public static async Task<TrackInfo> LoadAsync(Stream stream)
        {
            try
            {
                var document = await XDocument.LoadAsync(stream, LoadOptions.None, default);
                var trackInfoElement = document?.Root.Element("TrackInfo");

                if (trackInfoElement == null)
                    return null;

                var trackId = trackInfoElement.Attribute("ID")?.Value ?? string.Empty;
                var trackSummary = trackInfoElement.Element("TrackSummary");
                var track = new TrackInfo(trackId, new TrackDetail(trackSummary));

                foreach (var trackDetail in trackInfoElement.Elements("TrackDetail"))
                {
                    track.TrackDetails.Add(new TrackDetail(trackDetail));
                }

                return track;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
