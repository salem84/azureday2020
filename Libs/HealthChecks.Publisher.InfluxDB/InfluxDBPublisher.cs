using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.Publisher.InfluxDB
{
    public class InfluxDBPublisher : IHealthCheckPublisher
    {
        private const string HealthCheckUrl = "http://localhost:52494/hc";
        private const string InfluxdbWriteUrl = "http://grafanagio4rh5tuqwifp7k.northeurope.cloudapp.azure.com:8086/write?db=myk6db";
        private const string WebHostName = "gpf1";

        private readonly Func<HttpClient> _httpClientFactory;

        public InfluxDBPublisher(Func<HttpClient> httpClientFactory, InfluxDbOptions options)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            foreach (var keyedEntry in report.Entries)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var key = keyedEntry.Key;
                var entry = keyedEntry.Value;

                int status = ConvertStatus(entry.Status);
                var metric = new MetricInfluxDB()
                {
                    HostName = Environment.MachineName,
                    Service = key,
                    Status = status
                };

                await PushMetrics(metric);
            }
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

        private async Task PushMetrics(MetricInfluxDB metric)
        {
            try
            {
                var httpClient = _httpClientFactory();

                var pushMessage = new HttpRequestMessage(HttpMethod.Post, $"{InfluxdbWriteUrl}");
                var body = $"health,host={metric.HostName},service={metric.Service} value={metric.Status}";

                using (var stringContent = new StringContent(body, Encoding.UTF8))
                {
                    pushMessage.Content = stringContent;
                    (await httpClient.SendAsync(pushMessage))
                        .EnsureSuccessStatusCode();
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
            public int Status { get; set; }
        }
    }
}
