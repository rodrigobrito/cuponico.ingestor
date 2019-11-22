using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka
{
    public class AffiliateStoreKafkaConsumer
    {
        private readonly AffiliateStoreDomainService _domainService;
        private readonly ConsumerConfig _config;
        public AffiliateStoreKafkaConsumer(KafkaSettings settings, AffiliateStoreDomainService domainService)
        {
            _domainService = domainService.ThrowIfNull(nameof(domainService));
            settings.ThrowIfNull(nameof(settings));

            _config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = settings.Username,
                SaslPassword = settings.Password,
                ClientId = "cuponico.ingestor",
                GroupId = "cuponico.processor",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        }

        public void Start(CancellationToken cancellationToken)
        {
            StartReceivingCreatedStores(cancellationToken);
            StartReceivingChangedStores(cancellationToken);
            StartReceivingCanceledStores(cancellationToken);
        }

        private void StartReceivingCreatedStores(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(AffiliateStoreCreated.AffiliateEventName);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateStoreCreated = JsonConvert.DeserializeObject<AffiliateStoreCreated>(msg.Value);
                            _domainService.ProcessUnifiedStore(affiliateStoreCreated.Event).ConfigureAwait(false).GetAwaiter().GetResult();

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

        private void StartReceivingChangedStores(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(AffiliateStoreChanged.AffiliateEventName);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateStoreChanged = JsonConvert.DeserializeObject<AffiliateStoreChanged>(msg.Value);
                            _domainService.ProcessUnifiedStore(affiliateStoreChanged.Event).ConfigureAwait(false).GetAwaiter().GetResult();

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

        private void StartReceivingCanceledStores(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(AffiliateStoreCanceled.AffiliateEventName);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateStoreCanceled = JsonConvert.DeserializeObject<AffiliateStoreCanceled>(msg.Value);
                            _domainService.CancelUnifiedStore(affiliateStoreCanceled.Event).ConfigureAwait(false).GetAwaiter().GetResult();

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
