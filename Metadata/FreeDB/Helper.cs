using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace mor.FreeDB
{
    public class Helper
    {
        public Site MainSite { get; }
        public const string MAIN_FREEDB_ADDRESS = "freedb.freedb.org";
        public const string DEFAULT_ADDITIONAL_URL_INFO = "/~cddb/cddb.cgi";
        public string Version { get; set; }
        public string ClientName { get; set; }
        public string Hostname { get; set; }
        public string UserName { get; set; }
        public int ProtocolLevel { get; set; }
        public Site CurrentSite { get; set; }

        public Helper()
        {
            MainSite = new Site(MAIN_FREEDB_ADDRESS, Protocol.HTTP, DEFAULT_ADDITIONAL_URL_INFO);
            CurrentSite = MainSite; // default
            ProtocolLevel = 6; // default to level 6 support
        }

        public IEnumerable<Site> GetSites()
        {
            return GetSites(Protocol.CDDBP|Protocol.HTTP);
        }

        public IEnumerable<Site> GetSites(Protocol protocol)
        {
            var coll = Call("sites", MainSite.GetUrl());
            if (!coll.Any())
                throw new Exception("No results returned from sites request.", null);

            var sites = new List<Site>();
            var code = GetCode(coll);
            if (code == ResponseCode.CODE_210)
            {
                foreach (var line in coll.Skip(1))
                {
                    var site = new Site(line);
                    if ((protocol & site.Protocol) != 0)
                        sites.Append(site);
                }
                return sites;
            }
            throw new Exception($"Error Code: {code}");
        }

        public Entry Read(QueryResult result)
        {
            return Read(result.Category, result.Discid);
        }

        public Entry Read(string category, string discid)
        {
            var command = $"cddb+read+{category}+{discid}";
            var coll = Call(command);
            if (!coll.Any())
                throw new Exception("No results returned from cddb read.", null);

            var code = GetCode(coll);
            if (code == ResponseCode.CODE_210)
                return new Entry(coll.Skip(1)); // remove the 210 line

            throw new Exception($"Error Code: {code}");
        }


        /// <summary>
        /// Query the freedb server to see if there is information on this cd
        /// </summary>
        public IEnumerable<QueryResult> Query(string fingerprint)
        {
            var querystring = WebUtility.UrlEncode(fingerprint);
            var command = $"cddb+query+{querystring}";
            var coll = Call(command);
            if (!coll.Any())
                throw new Exception("No results returned from cddb query.", null);

            var code = GetCode(coll);
            switch (code)
            {
                // Multiple results
                case ResponseCode.CODE_211:
                case ResponseCode.CODE_210:
                    return coll
                        .Skip(1)
                        .Select( x => new QueryResult(x));

                // Exact match 
                case ResponseCode.CODE_200:
                    return coll
                        .Take(1)
                        .Select( x => new QueryResult(x));
            }
            throw new Exception($"Error code: {code}");
        }


        /// <summary>
        /// Retrieve the categories
        /// </summary>
        public IEnumerable<String> GetCategories()
        {
            var coll = Call("cddb+lscat");
            if (!coll.Any())
                throw new Exception("No results returned from categories request.", null);

            var code = GetCode(coll);
            if (code == ResponseCode.CODE_210)
                return coll.Skip(1);

            throw new Exception($"Error code: {code}");
        }

        private IEnumerable<String> Call(string command)
        {
            return Call(command, CurrentSite.GetUrl());
        }

        // make the call to the service
        private IEnumerable<String> Call(string commandIn, string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "text/plain";
            req.Method = "POST";

            var command = BuildCommand($"cmd={commandIn}");
            byte[] byteArray = Encoding.UTF8.GetBytes(command);
            using (var newStream = req.GetRequestStream())
            {
                newStream.Write(byteArray, 0, byteArray.Length);
            }

            var coll = new List<String>();
            using (var response = (HttpWebResponse)req.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream(),
                    System.Text.Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("."))
                            break;
                        coll.Add(line);
                    }
                }
            }
            return coll;
        }

        private string BuildCommand(string command)
        {
            return $"{command}&{Hello()}&{Proto()}";
        }

        public string Hello()
            => $"hello={UserName}+{Hostname}+{ClientName}+{Version}";

        public string Proto()
            => $"proto={ProtocolLevel}";

        private ResponseCode GetCode(IEnumerable<string> lines)
        {
            return GetCode(lines.First());
        }
        private ResponseCode GetCode(string line)
        {
            var regex = new Regex(@"^(\d{3})\s");
            var m = regex.Match(line);
            if (!m.Success)
                throw new Exception($"Unable to process results for cddb read. Returned Data: {line}", null);

            var code = m.Groups[1].Value;
            return (ResponseCode)Enum.Parse(typeof(ResponseCode), $"CODE_{code}");
        }
    }
}
