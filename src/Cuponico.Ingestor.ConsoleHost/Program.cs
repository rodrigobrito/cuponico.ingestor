using System;
using System.Threading;
using Coravel;
using Cuponico.Ingestor.Host.Partners.Lomadee.Jobs;
using Microsoft.AspNetCore.Hosting;

namespace Cuponico.Ingestor.Host
{
    internal class Program
    {
        private static void Main()
        {
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
                scheduler.OnWorker("Lomadee.Categories.Importer");
                scheduler.Schedule<LomadeeCategoriesSchedulableJob>().EveryMinute();

                scheduler.OnWorker("Lomadee.Stores.Importer");
                scheduler.Schedule<LomadeeStoresSchedulableJob>().EveryMinute();

                scheduler.OnWorker("Lomadee.Coupons.Importer");
                scheduler.Schedule<LomadeeCouponsSchedulableJob>().EveryMinute();
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