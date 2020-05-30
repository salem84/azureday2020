using System;
using System.Collections.Generic;
using System.Text;

namespace HealthChecks.Publisher.InfluxDB
{
    public class InfluxDbOptions
    {
        public string WriteApiUrl { get; set; }
        public string DatabaseName { get; set; }
    }
}
