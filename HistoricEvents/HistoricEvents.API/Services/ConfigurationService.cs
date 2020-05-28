using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace peopleapi.Services
{
    public class ConfigurationService
    {
        private static Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();
        private string key = "CHIAVE";
        private string _bussolaStream;

        public string Header;
        public string Body;

        public ConfigurationService()
        {
            if (_cache.ContainsKey(key))
            {
                _bussolaStream = _cache[key].Value as string;
            }
            else
            {
                _bussolaStream = CreabussolaStream();
                _cache.Add(key, new CacheItem(key, _bussolaStream));
            }


            if (!string.IsNullOrEmpty(_bussolaStream))
            {
                //
                // Recupera dalla string di output tutto ciò che è contenuto tra i tag <head></head>
                //
                int startIndex = _bussolaStream.IndexOf("<head>") + 6;

                int endIndex = _bussolaStream.IndexOf("</head>");

                this.Header = _bussolaStream.Substring(startIndex, endIndex - startIndex);

                //
                // Recupera dalla string di output tutto ciò che è contenuto tra i tag <body><div id="mainDiv">
                //
                startIndex = _bussolaStream.IndexOf("<body>") + 6;

                endIndex = _bussolaStream.IndexOf("</footer>");

                this.Body = _bussolaStream.Substring(startIndex, endIndex - startIndex);
            }
        }

        private string CreabussolaStream()
        {
            string bussola = string.Empty;
            try
            {
                return File.ReadAllText("page.html");
            }
            catch (Exception ex)
            {
                // ignored
            }
            return bussola;
        }
    }

    public class CacheItem
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string RegionName { get; set; }
        private CacheItem()
        {
        }
        public CacheItem(string key)
        {
            Key = key;
        }
        public CacheItem(string key, object value)
        : this(key)
        {
            Value = value;
        }
    }
}
