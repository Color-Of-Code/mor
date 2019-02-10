using System;
using System.Text.RegularExpressions;

namespace mor.FreeDB
{
    public class QueryResult
    {
        public string Category { get; set; }

        public string Discid { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public QueryResult(string queryResult)
        {
            Parse(queryResult);
        }

        public bool Parse(string result)
        {
            // the eventual code part (\d{3}\s+)? is ignored
            var r = new Regex(@"^(\d{3}\s+)?(?<category>\S+)\s+(?<discid>[a-f0-9]{8})\s+(?<artist>.*?)\s+/\s+(?<title>.*)$");
            var m = r.Match(result);
            if (!m.Success)
                throw new Exception("Unable to Parse QueryResult. Input: " + result);

            var groups = m.Groups;

            Category = groups["category"].Value;
            Discid = groups["discid"].Value;
            Artist = groups["artist"].Value;
            Title = groups["title"].Value;
            return true;
        }

        public override string ToString() => $"{Artist}, {Title}";
    }
}
