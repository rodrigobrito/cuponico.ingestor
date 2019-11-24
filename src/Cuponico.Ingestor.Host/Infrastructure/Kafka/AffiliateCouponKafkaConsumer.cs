using Confluent.Kafka;
using Elevar.Utils;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Settings;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka
{
    public class AffiliateCouponKafkaConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly AffiliateCouponDomainService _domainService;
        public AffiliateCouponKafkaConsumer(KafkaSettings settings, AffiliateCouponDomainService domainService)
        {
            _domainService = domainService.ThrowIfNull(nameof(domainService));

            settings.ThrowIfNull(nameof(settings));

            _config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                HeartbeatIntervalMs = 3000,
                SaslUsername = settings.Username,
                SaslPassword = settings.Password,
                ClientId = settings.ClientId,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        }

        public void Start(CancellationToken cancellationToken)
        {
            StartReceivingCreatedCoupons(cancellationToken);
            StartReceivingChangedCoupons(cancellationToken);
            StartReceivingCanceledCoupons(cancellationToken);
        }

        private void StartReceivingCreatedCoupons(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(CuponicoEvents.AffiliateCouponCreated);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateCouponCreated = JsonConvert.DeserializeObject<AffiliateCouponCreated>(msg.Value);
                            _domainService.ProcessCoupon(affiliateCouponCreated.Event).ConfigureAwait(false).GetAwaiter().GetResult();
                            consumer.Commit();
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                        catch (OperationCanceledException e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                    }
                    consumer.Close();
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void StartReceivingChangedCoupons(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(CuponicoEvents.AffiliateCouponChanged);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateCouponChanged = JsonConvert.DeserializeObject<AffiliateCouponChanged>(msg.Value);
                            _domainService.ProcessCoupon(affiliateCouponChanged.Event).ConfigureAwait(false).GetAwaiter().GetResult();

                            consumer.Commit();
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                        catch (OperationCanceledException e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                    }
                    consumer.Close();
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void StartReceivingCanceledCoupons(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(CuponicoEvents.AffiliateCouponCanceled);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateCouponCanceled = JsonConvert.DeserializeObject<AffiliateCouponCanceled>(msg.Value);
                            _domainService.CancelCoupon(affiliateCouponCanceled.Event);

                            consumer.Commit();
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                        catch (OperationCanceledException e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                    }
                    consumer.Close();
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
