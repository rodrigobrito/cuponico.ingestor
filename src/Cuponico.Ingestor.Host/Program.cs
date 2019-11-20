using System;
using System.Threading;
using Coravel;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Lomadee;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Zanox;
using Microsoft.AspNetCore.Hosting;

namespace Cuponico.Ingestor.Host
{
    internal class Program
    {
        private static void Main()
        {
            //var couponAvro = AvroSerializer.Create<CouponId>().WriterSchema.ToString();
            Console.WriteLine("Starting...");

            var cancelationTokenSource = new CancellationTokenSource();
            var cancelToken = cancelationTokenSource.Token;

            var host = new WebHostBuilder()
                            .UseIISIntegration()
                            .UseKestrel()
                            .UseStartup<Startup>()
                            .Build();

            var services = host.Services;
            services.UseScheduler(scheduler =>
            {
                // Lomadee
                scheduler.Schedule<AffiliateCouponsSchedulableJobLomadee>().EveryFiveMinutes()
                    .PreventOverlapping(nameof(AffiliateCouponsSchedulableJobLomadee));

                scheduler.Schedule<AffiliateStoresSchedulableJobLomadee>().EveryFifteenMinutes()
                    .PreventOverlapping(nameof(AffiliateStoresSchedulableJobLomadee));

                scheduler.Schedule<AffiliateCategoriesSchedulableJobLomadee>().EveryFifteenMinutes()
                    .PreventOverlapping(nameof(AffiliateCategoriesSchedulableJobLomadee));

                // Zanox
                scheduler.Schedule<AffiliateCouponsSchedulableJobZanox>().EveryFiveMinutes()
                    .PreventOverlapping(nameof(AffiliateCouponsSchedulableJobZanox));

                scheduler.Schedule<AffiliateStoresSchedulableJobZanox>().EveryFifteenMinutes()
                    .PreventOverlapping(nameof(AffiliateStoresSchedulableJobZanox));

                scheduler.Schedule<AffiliateCategoriesSchedulableJobZanox>().EveryFifteenMinutes()
                    .PreventOverlapping(nameof(AffiliateCategoriesSchedulableJobZanox));
            });

            host.Start();

            Console.WriteLine("Started.");

            Console.CancelKeyPress += (_, e) =>
            {
                Console.WriteLine("Cancelling...");

                e.Cancel = true;
                cancelationTokenSource.Cancel();
            };
            WaitHandle.WaitAny(new[] { cancelToken.WaitHandle });
        }
    }
}