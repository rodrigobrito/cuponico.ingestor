using AutoMapper;
using Coravel;
using Cuponico.Ingestor.Host.Domain.Jobs;
using Cuponico.Ingestor.Host.Infrastructure.Health;
using Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee;
using Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Categories;
using Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Stores;
using Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.Zanox;
using Cuponico.Ingestor.Host.Infrastructure.Kafka;
using Cuponico.Ingestor.Host.Infrastructure.Kafka.Coupons;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
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
using Polly.Retry;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Medias;
using Cuponico.Ingestor.Host.Infrastructure.Http.Zanox.Programs;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Lomadee;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Zanox;
using Coupon = Cuponico.Ingestor.Host.Infrastructure.Kafka.Coupons.Coupon;

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

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error != null)
                        Console.WriteLine(exceptionHandlerPathFeature.Error);
                });
            });
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // Add access to generic IConfigurationRoot
            services.AddSingleton(Configuration);

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
            services.AddScheduler();

            services.AddHealthChecks()
                    .AddGcInfoCheck("services");
            
            // Add Kafka
            services.AddSingleton<KafkaSettings>();
            services.AddSingleton<KafkaProducer<CouponKey, Coupon>>();

            // Add http policies
            var jitterer = new Random();
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                                                  .WaitAndRetryAsync(5, retryAttempt =>
                                                                     TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                                                     TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));

            ConfigureLomadee(services, retryPolicy);
            ConfigureZanox(services, retryPolicy);
        }

        private void ConfigureZanox(IServiceCollection services, AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            var zanoxSettings = new ZanoxSettings(Configuration);
            services.AddSingleton(zanoxSettings.Mongo);
            services.AddSingleton(zanoxSettings.Http);

            services.AddHttpClient<ZanoxProgramHttpRepository>(c => { c.BaseAddress = new Uri(zanoxSettings.Http.BaseUrl); })
                    .AddPolicyHandler(retryPolicy);

            // Stores
            services.AddHttpClient<ZanoxStoreHttpRepository>(c => { c.BaseAddress = new Uri(zanoxSettings.Http.BaseUrl); })
                    .AddPolicyHandler(retryPolicy);

            services.AddSingleton<ZanoxStoreMongoDbRepository>();
            services.AddTransient(provider => new StoresSchedulableJobZanox(
                provider.GetService<ZanoxStoreHttpRepository>(),
                provider.GetService<ZanoxStoreMongoDbRepository>()));
        }

        private void ConfigureLomadee(IServiceCollection services, AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            var lomadeeSettings = new LomadeeSettings(Configuration);
            services.AddSingleton(lomadeeSettings.Mongo);
            services.AddSingleton(lomadeeSettings.Http);


            // Add AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(typeof(LomadeeCouponProfile));
                cfg.AddProfile(typeof(LomadeeStoreProfile));
                cfg.AddProfile(typeof(LomadeeCategoryProfile));
            }, AppDomain.CurrentDomain.GetAssemblies());


            // Coupons
            services.AddHttpClient<LomadeeeCouponHttpRepository>(c => { c.BaseAddress = new Uri(lomadeeSettings.Http.Host); })
                .AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeCouponMongoDbRepository>();
            services.AddTransient(provider => new CouponsSchedulableJobLomadee(
                provider.GetService<LomadeeeCouponHttpRepository>(),
                provider.GetService<LomadeeCouponMongoDbRepository>()));

            // Stores
            services.AddHttpClient<LomadeeStoreHttpRepository>(c => { c.BaseAddress = new Uri(lomadeeSettings.Http.Host); })
                    .AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeStoreMongoDbRepository>();
            services.AddTransient(provider => new StoresSchedulableJobLomadee(
                provider.GetService<LomadeeStoreHttpRepository>(),
                provider.GetService<LomadeeStoreMongoDbRepository>()));

            // Categories
            services.AddHttpClient<LomadeeCategoryHttpRepository>(c => { c.BaseAddress = new Uri(lomadeeSettings.Http.Host); })
                    .AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeCategoryMongoDbRepository>();
            services.AddTransient(provider => new CategoriesSchedulableJobLomadee(
                provider.GetService<LomadeeCategoryHttpRepository>(),
                provider.GetService<LomadeeCategoryMongoDbRepository>()));
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
