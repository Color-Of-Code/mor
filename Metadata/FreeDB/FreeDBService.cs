
using System.Collections.Generic;
using mor.FreeDB;

namespace mor.Metadata
{

    public class FreeDBService
    {
        private Helper _helper;
        public FreeDBService(string mainService = "")
        {
            _helper = new Helper();
            _helper.UserName = "color-of-code";
            _helper.ClientName = "mor";
            _helper.Version = "0.1";
            _helper.Hostname = "jdh";
        }

        public string GetSites()
        {
            var sites = _helper.GetSites();
            return "";
        }

        public IEnumerable<string> GetCategories()
        {
            return _helper.GetCategories();
        }

        public IEnumerable<QueryResult> Query(string fingerprint)
        {
            return _helper.Query(fingerprint);
        }

        public Entry Read(QueryResult result)
        {
            return _helper.Read(result);
        }
    }
}
