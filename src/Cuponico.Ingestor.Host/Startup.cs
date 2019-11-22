using AutoMapper;
using Coravel;
using Cuponico.Ingestor.Host.Infrastructure.Health;
using Cuponico.Ingestor.Host.Infrastructure.Kafka;
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
using Cuponico.Ingestor.Host.Domain.Advertiser.Stores;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Lomadee;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Zanox;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Categories;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Stores;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Lomadee.Coupons.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Incentives;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Medias;
using Cuponico.Ingestor.Host.Infrastructure.Http.AffiliatePrograms.Zanox.Programs;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Stores;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox;

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
#pragma warning disable 1998
                errorApp.Run(async context =>
#pragma warning restore 1998
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
            services.AddSingleton<KafkaBus>();

            // Add http policies
            var jitterer = new Random();
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                                                  .WaitAndRetryAsync(5, retryAttempt =>
                                                                     TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                                                     TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));

            services.AddSingleton<AdvertiserMongoSettings>();


            ConfigureLomadee(services, retryPolicy);
            ConfigureZanox(services, retryPolicy);
            ConfigureAdvertiser(services);
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
            services.AddTransient(provider => new AffiliateStoresSchedulableJobZanox(
                provider.GetService<ZanoxStoreHttpRepository>(),
                provider.GetService<ZanoxStoreMongoDbRepository>(),
                provider.GetService<KafkaBus>()));

            // Categories
            services.AddSingleton<ZanoxCategoryHttpRepository>();

            services.AddSingleton<ZanoxCategoryMongoDbRepository>();
            services.AddTransient(provider => new AffiliateCategoriesSchedulableJobZanox(
                provider.GetService<ZanoxCategoryHttpRepository>(),
                provider.GetService<ZanoxCategoryMongoDbRepository>(),
                provider.GetService<KafkaBus>()));

            // Coupons 
            services.AddHttpClient<ZanoxCouponRepository>(c => { c.BaseAddress = new Uri(zanoxSettings.Http.BaseUrl); })
                    .AddPolicyHandler(retryPolicy);

            services.AddSingleton<ZanoxCouponMongoDbRepository>();
            services.AddTransient(provider => new AffiliateCouponsSchedulableJobZanox(
                provider.GetService<ZanoxCouponRepository>(),
                provider.GetService<ZanoxCouponMongoDbRepository>()));
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
            services.AddTransient(provider => new AffiliateCouponsSchedulableJobLomadee(
                provider.GetService<LomadeeeCouponHttpRepository>(),
                provider.GetService<LomadeeCouponMongoDbRepository>()));

            // Stores
            services.AddHttpClient<LomadeeStoreHttpRepository>(c => { c.BaseAddress = new Uri(lomadeeSettings.Http.Host); })
                    .AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeStoreMongoDbRepository>();
            services.AddTransient(provider => new AffiliateStoresSchedulableJobLomadee(
                provider.GetService<LomadeeStoreHttpRepository>(),
                provider.GetService<LomadeeStoreMongoDbRepository>(),
                provider.GetService<KafkaBus>()));

            // Categories
            services.AddHttpClient<LomadeeCategoryHttpRepository>(c => { c.BaseAddress = new Uri(lomadeeSettings.Http.Host); })
                    .AddPolicyHandler(retryPolicy);
            services.AddSingleton<LomadeeCategoryMongoDbRepository>();
            services.AddTransient(provider => new AffiliateCategoriesSchedulableJobLomadee(
                provider.GetService<LomadeeCategoryHttpRepository>(),
                provider.GetService<LomadeeCategoryMongoDbRepository>(),
                provider.GetService<KafkaBus>()));
        }

        private void ConfigureAdvertiser(IServiceCollection services)
        {
            services.AddSingleton<AdvertiserMongoSettings>();
            services.AddSingleton<StoreMongoDbRepository>();
            services.AddSingleton<IStoreRepository>(provider => (StoreMongoDbRepository)provider.GetService(typeof(StoreMongoDbRepository)));
            services.AddSingleton<AffiliateStoreMatchesMongoDbRepository>();
            services.AddSingleton<IAffiliateStoreMatchesRepository>(provider => (AffiliateStoreMatchesMongoDbRepository)provider.GetService(typeof(AffiliateStoreMatchesMongoDbRepository)));
            services.AddSingleton<AffiliateStoreDomainService>();
            services.AddSingleton<AffiliateStoreKafkaConsumer>();
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
