using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.Publisher.InfluxDB
{
    public class InfluxDBPublisher : IHealthCheckPublisher
    {

        private readonly Func<HttpClient> _httpClientFactory;
        private readonly InfluxDbOptions _options;

        public InfluxDBPublisher(Func<HttpClient> httpClientFactory, InfluxDbOptions options)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var metrics = new List<MetricInfluxDB>();
            foreach (var keyedEntry in report.Entries)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var key = keyedEntry.Key;
                var entry = keyedEntry.Value;

                int status = ConvertStatus(entry.Status);
                
                if(entry.Data.Any())
                {
                    foreach (var data in entry.Data)
                    {
                        var value = Convert.ToDecimal(data.Value);
                        
                        metrics.Add(new MetricInfluxDB()
                        {
                            HostName = Environment.MachineName,
                            Service = data.Key,
                            Value = value
                        });
                    }
                }
                else
                {
                    metrics.Add(new MetricInfluxDB()
                    {
                        HostName = Environment.MachineName,
                        Service = key,
                        Value = status
                    });
                }
                

            }

            await PushMetrics(metrics);

        }

        private static int ConvertStatus(HealthStatus healthStatus)
        {
            int status = 2;
            switch (healthStatus)
            {
                case HealthStatus.Degraded:
                    status = 1;
                    break;
                case HealthStatus.Unhealthy:
                    status = 0;
                    break;
                case HealthStatus.Healthy:
                default:
                    status = 2;
                    break;
            }

            return status;
        }

        //private async Task PushMetrics(MetricInfluxDB metric)
        //{
        //    try
        //    {
        //        var httpClient = _httpClientFactory();

        //        var pushMessage = new HttpRequestMessage(HttpMethod.Post, $"{InfluxdbWriteUrl}");
        //        var body = $"health,host={metric.HostName},service={metric.Service} value={metric.Status}";

        //        using (var stringContent = new StringContent(body, Encoding.UTF8))
        //        {
        //            pushMessage.Content = stringContent;
        //            (await httpClient.SendAsync(pushMessage))
        //                .EnsureSuccessStatusCode();
        //        }
                   
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine($"Exception is throwed publishing metrics with message: {ex.Message}");
        //    }
        //}

        private async Task PushMetrics(IEnumerable<MetricInfluxDB> metrics)
        {
            try
            {
                var httpClient = _httpClientFactory();

                foreach (var metric in metrics)
                {
                    var pushMessage = new HttpRequestMessage(HttpMethod.Post, $"{_options.WriteApiUrl}?db={_options.DatabaseName}");
                    
                    var body = $"health,host={metric.HostName},service={metric.Service} value={metric.Value}";

                    using (var stringContent = new StringContent(body, Encoding.UTF8))
                    {
                        pushMessage.Content = stringContent;
                        (await httpClient.SendAsync(pushMessage))
                            .EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Exception is throwed publishing metrics with message: {ex.Message}");
            }
        }

        class MetricInfluxDB
        {
            public string HostName { get; set; }
            public string Service { get; set; }
            public decimal Value { get; set; }
        }


        //private string ConvertToInfluxDbFormat(MetricInfluxDB metric)
        //{
        //    var sb = new StringBuilder(metric.Measurement).Append(",");

        //    foreach (var p in metric.Properties)
        //    {
        //        sb.Append(p.Key).Append("=").Append(p.Value);
        //    }

        //    sb.Append(" value=").Append(metric.Value);
        //    return sb.ToString();
        //}

        //class MetricInfluxDB
        //{
        //    public string Measurement { get; set; }
        //    public decimal Value { get; set; }
        //    public Dictionary<string, decimal> Properties { get; set; }
        //}
    }
}
