using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading;
using Coravel;
using Ingestor.ConsoleHost.Partners.Lomadee.Jobs;

namespace Ingestor.ConsoleHost
{
    internal class Program
    {
        private static void Main()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var cancelToken = cancelationTokenSource.Token;

            var host = new WebHostBuilder()
                            .UseStartup<Startup>()
                            .UseKestrel()
                            .Build();

            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<LomadeeCategoriesSchedulableJob>().EveryMinute();
            });
            host.Start();

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