using System;
using System.Threading;
using Coravel;
using Cuponico.Ingestor.Host.Domain.Jobs.Lomadee;
using Cuponico.Ingestor.Host.Domain.Jobs.Zanox;
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
                //scheduler.Schedule<CouponsSchedulableJobLomadee>().EveryFiveMinutes()
                //    .PreventOverlapping(nameof(CouponsSchedulableJobLomadee));

                //scheduler.Schedule<StoresSchedulableJobLomadee>().EveryFifteenMinutes()
                //    .PreventOverlapping(nameof(StoresSchedulableJobLomadee));

                //scheduler.Schedule<CategoriesSchedulableJobLomadee>().EveryFifteenMinutes()
                //    .PreventOverlapping(nameof(CategoriesSchedulableJobLomadee));

                // Zanox
                //scheduler.Schedule<CouponsSchedulableJobZanox>().EverySecond()
                //    .PreventOverlapping(nameof(CouponsSchedulableJobZanox));

                scheduler.Schedule<StoresSchedulableJobZanox>().EverySecond()
                    .PreventOverlapping(nameof(StoresSchedulableJobZanox));

                //scheduler.Schedule<CategoriesSchedulableJobZanox>().EveryFifteenMinutes()
                //    .PreventOverlapping(nameof(CategoriesSchedulableJobZanox));
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