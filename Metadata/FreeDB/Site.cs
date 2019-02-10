using System;
using System.Text.RegularExpressions;

namespace mor.FreeDB
{
    public class Site
    {
        /// <summary>
        /// Any additional addressing information needed to access the server.
        /// For example, for HTTP protocol servers, this would be the path to the CCDB server CGI script.
        /// This field will be "-" if no additional addressing information is needed.
        /// </summary>
        public string AdditionalAddressInfo { get; set; }

        /// <summary>
        /// Internet address of the remote site 
        /// </summary>
        public string SiteAddress { get; set; }

        /// <summary>
        /// The transfer protocol used to access the site
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// The port at which the server resides on that site.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// A short description of the geographical location of the site.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The latitude of the server site. The format is as follows:
        /// CDDD.MM
        /// Where "C" is the compass direction (N, S), "DDD" is the
        /// degrees, and "MM" is the minutes.
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// The longitude of the server site. Format is as above, except
        /// the compass direction must be one of (E, W).
        /// </summary>
        public string Longitude { get; set; }

        public Site(string site)
        {
            Parse(site);
        }

        /// <summary>
        /// Builds a site from an address, protocol and addition info
        /// </summary>
        public Site(string siteAddress, Protocol protocol, string additionAddressInfo)
        {
            SiteAddress = siteAddress;
            Protocol = protocol;
            AdditionalAddressInfo = additionAddressInfo;
        }

        public void Parse(string site)
        {
            var r = new Regex(@"^(?<address>\S+)\s+(?<protocol>\S+)\s+(?<port>\d+)\s+(?<info>\S+)\s+(?<lat>\S+)\s+(?<long>\S+)\s+(?<description>.*)$");
            var m = r.Match(site);
            if (!m.Success)
                throw new Exception("Unable to Parse Site. Input: " + site);
            SiteAddress = m.Groups["address"].Value;
            var p = m.Groups["protocol"].Value;
            Protocol = (Protocol)Enum.Parse(typeof(Protocol), p.ToUpperInvariant());
            Port = int.Parse(m.Groups["port"].Value);
            AdditionalAddressInfo = m.Groups["info"].Value;
            Latitude = m.Groups["lat"].Value;
            Longitude = m.Groups["long"].Value;
            Description = m.Groups["description"].Value;
        }

        public string GetUrl()
        {
            if (Protocol == Protocol.HTTP)
                return "http://" + SiteAddress + AdditionalAddressInfo;
            else
                return SiteAddress;
        }

        public override string ToString() => $"{SiteAddress}, {Description}";

    }
}
