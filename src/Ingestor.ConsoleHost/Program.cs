using Ingestor.ConsoleHost.Partners.Lomadee;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ingestor.ConsoleHost
{
    internal class Program
    {
        private static async Task Main()
        {
            var host = new WebHostBuilder()
                            .UseStartup<Startup>()
                            .UseKestrel()
                            .Build();
            host.Start();
            await Start(host.Services);
        }

        private static async Task Start(IServiceProvider services)
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var cancelToken = cancelationTokenSource.Token;

            var categoriesImporter = services.GetService<CouponsCategoryHttpToMongoDb>();
            await categoriesImporter.Import();

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
