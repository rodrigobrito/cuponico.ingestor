using System;
using System.IO;
using Ingestor.ConsoleHost.Partners.Lomadee;
using Ingestor.ConsoleHost.Partners.Lomadee.Http.Coupons.Categories;
using Ingestor.ConsoleHost.Partners.Lomadee.MongoDb.Coupons.Categories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Ingestor.ConsoleHost
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
            services.AddTransient<CouponsCategoryHttpToMongoDb>();
        }

        public IConfigurationRoot Configuration { get; set; }
    }
}
