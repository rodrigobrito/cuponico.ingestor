﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Cuponico.Ingestor.Host.Infrastructure.Health
{
    public static class GcInfoHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddGcInfoCheck(
            this IHealthChecksBuilder builder,
            string name,
            HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null,
            long? thresholdInBytes = null)
        {
            // Register a check of type GCInfo
            builder.AddCheck<GcInfoHealthCheck>(name, failureStatus ?? HealthStatus.Degraded, tags);

            // Configure named options to pass the threshold into the check.
            if (thresholdInBytes.HasValue)
            {
                builder.Services.Configure<GcInfoOptions>(name, options =>
                {
                    options.Threshold = thresholdInBytes.Value;
                });
            }

            return builder;
        }
    }

    public class GcInfoHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<GcInfoOptions> _options;

        public GcInfoHealthCheck(IOptionsMonitor<GcInfoOptions> options)
        {
            _options = options;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = _options.Get(context.Registration.Name);

            // This example will report degraded status if the application is using
            // more than the configured amount of memory (1gb by default).
            //
            // Additionally we include some GC info in the reported diagnostics.
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var data = new Dictionary<string, object>()
            {
                { "Allocated", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) },
            };

            // Report failure if the allocated memory is >= the threshold.
            //
            // Using context.Registration.FailureStatus means that the application developer can configure
            // how they want failures to appear.
            var result = allocated >= options.Threshold ? context.Registration.FailureStatus : HealthStatus.Healthy;

            return Task.FromResult(new HealthCheckResult(
                result,
                "Reports degraded status if allocated bytes >= 1gb",
                data: data));
        }
    }

    public class GcInfoOptions
    {
        // The failure threshold (in bytes)
        public long Threshold { get; set; } = 1024L * 1024L * 1024L;
    }
}
