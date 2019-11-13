using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Coravel;
using Cuponico.Ingestor.Host.Health;
using Cuponico.Ingestor.Host.Kafka;
using Cuponico.Ingestor.Host.Partners.Coupons;
using Cuponico.Ingestor.Host.Partners.Lomadee;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Categories;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Stores;
using Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Tickets;
using Cuponico.Ingestor.Host.Partners.Lomadee.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Extensions.Http;

namespace Cuponico.Ingestor.Host
{
    internal class Startup
    {
        public Startup()
        {
            // Build configuration
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/", new HealthCheckOptions
            {
                // The following StatusCodes are the default assignments for
                // the HealthStatus properties.
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = WriteResponse
            });
            app.UseMvc();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // Add access to generic IConfigurationRoot
            services.AddSingleton(Configuration);

            // Add AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(typeof(LomadeeCouponProfile));
            }, AppDomain.CurrentDomain.GetAssemblies());

            // Add logging
            services.AddLogging((logging) =>
            {
                logging.AddConfiguration(Configuration.GetSection("Logging"));
                logging.AddConsole();
            });

            // Health checks
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddMvc();

            services.AddHealthChecks()
                    .AddGcInfoCheck("services");

            // Adding scheduler
            services.AddScheduler();

            // Add Kafka
            services.AddSingleton<KafkaSettings>();
            services.AddSingleton<KafkaProducer<CouponKey, Coupon>>();

            // Add http policies
            var jitterer = new Random();
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                                                  .WaitAndRetryAsync(5, retryAttempt =>
                                                                     TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                                                     TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));

            var lomadeeSettings = new LomadeeSettings(Configuration);
            services.AddSingleton(lomadeeSettings.Mongo);
            services.AddSingleton(lomadeeSettings.Http);

            // Categories
            services.AddHttpClient<LomadeeCategoryHttpRepository>(c =>
            {
                c.BaseAddress = new Uri(lomadeeSettings.Http.Host);
            }).AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeCategoryMongoDbRepository>();
            services.AddTransient<LomadeeCategoriesSchedulableJob>();

            // Stores
            services.AddHttpClient<LomadeeStoreHttpRepository>(c =>
            {
                c.BaseAddress = new Uri(lomadeeSettings.Http.Host);
            }).AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeStoreMongoDbRepository>();
            services.AddTransient<LomadeeStoresSchedulableJob>();

            // Coupons
            services.AddHttpClient<LomadeeeCouponHttpRepository>(c =>
            {
                c.BaseAddress = new Uri(lomadeeSettings.Http.Host);
            }).AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeCouponMongoDbRepository>();
            services.AddTransient<LomadeeCouponsSchedulableJob>();
        }

        public IConfigurationRoot Configuration { get; set; }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}
