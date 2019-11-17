using System;
using System.Threading;
using Coravel;
using Cuponico.Ingestor.Host.Domain.Jobs;
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
                scheduler.Schedule<CouponsSchedulableJobLomadee>().EveryMinute()
                    .PreventOverlapping(nameof(CouponsSchedulableJobLomadee));

                scheduler.Schedule<StoresSchedulableJobLomadee>().EveryMinute()
                    .PreventOverlapping(nameof(StoresSchedulableJobLomadee));

                scheduler.Schedule<CategoriesSchedulableJobLomadee>().EveryMinute()
                    .PreventOverlapping(nameof(CategoriesSchedulableJobLomadee));


                //scheduler.OnWorker("Zanox.Stores.Importer");
                //scheduler.Schedule<ZanoxStoresSchedulableJob>().EveryMinute()
                //                                               .PreventOverlapping(nameof(ZanoxStoresSchedulableJob));
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