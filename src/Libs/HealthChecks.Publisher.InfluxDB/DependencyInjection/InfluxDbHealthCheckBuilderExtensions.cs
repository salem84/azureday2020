using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace HealthChecks.Publisher.InfluxDB.DependencyInjection
{
    public static class InfluxDbHealthCheckBuilderExtensions
    {
        const string NAME = "influxdb";
        public static IHealthChecksBuilder AddInfluxDbPublisher(this IHealthChecksBuilder builder, Action<InfluxDbOptions> setup, string name = default)
        {
            builder.Services.AddHttpClient();

            var options = new InfluxDbOptions();
            setup?.Invoke(options);

            var registrationName = name ?? NAME;

            builder.Services.AddSingleton<IHealthCheckPublisher>(sp =>
            {
                return new InfluxDBPublisher(() => sp.GetRequiredService<IHttpClientFactory>().CreateClient(registrationName), options);
            });

            return builder;
        }
    }
}
