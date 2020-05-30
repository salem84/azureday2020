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
            if (_cache.Count == 0)
            {
                _cache.Add("TimerConfiguration", new CacheItem("TimerConfiguration", 10));
            }
           


           
        }

        public CacheItem Get(string key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            return null;
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
