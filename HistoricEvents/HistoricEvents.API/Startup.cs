using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Food.API.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using HistoricEvents.API.Utility;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.Publisher.InfluxDB.DependencyInjection;
using HealthChecks.Publisher.InfluxDB;
using HealthChecks.UI.Client;
using HistoricEvents.API.Data;
using HistoricEvents.API.Utility.HealthCheck;

namespace Food.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration.GetConnectionString("EventsDatabaseConnString");

            services.AddOptions();

            services.AddDbContext<EventsDbContext>(opt =>
            {
                if (string.IsNullOrEmpty(connString))
                {
                    opt.UseInMemoryDatabase("EventsDatabase");
                }
                else
                {
                    opt.UseSqlServer(connString);
                }
            });

            #region HEALTHCHECKS

            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("API", "/health");
            });

            var healthChecksBuilder = services.AddHealthChecks();

            healthChecksBuilder.AddCheck("Foo", () => HealthCheckResult.Healthy("Foo is OK!"), tags: new[] { "foo_tag" })
                .AddCheck("Bar", () => HealthCheckResult.Unhealthy("Bar is unhealthy!"), tags: new[] { "bar_tag" })
                .AddCheck("Baz", () => HealthCheckResult.Healthy("Baz is OK!"), tags: new[] { "baz_tag" })
                .AddMemoryHealthCheck("memory", thresholdInBytes: 1024L * 1024L * 200L)
                .AddInfluxDbPublisher(x => new InfluxDbOptions());

            if (!string.IsNullOrEmpty(connString))
            {
                healthChecksBuilder.AddSqlServer(
                    connString,
                    name: "EventsDB-check");
            }

            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });


            services.AddControllers()
                   .AddNewtonsoftJson(options =>
                       options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });


            services.AddOptions()
                .Configure<ErrorSimulatorOptions>(Configuration.GetSection("ErrorSimulator"))
                .Configure<DelaySimulatorOptions>(Configuration.GetSection("DelaySimulator"));

            services.AddTransient<ErrorSimulatorFilter>();
            services.AddTransient<DelaySimulatorFilter>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            ILoggerFactory loggerFactory, 
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (errorFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                        }

                        await context.Response.WriteAsync("There was an error");
                    });
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    //ResponseWriter = HealthCheckHelpers.WriteResponse
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI(setup =>
                {
                    
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }



    }
}
