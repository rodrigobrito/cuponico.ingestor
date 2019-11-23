using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Infrastructure.Settings;
using Elevar.Utils;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka
{
    public class AffiliateCategoryKafkaConsumer
    {
        private readonly AffiliateCategoryDomainService _domainService;
        private readonly ConsumerConfig _config;
        public AffiliateCategoryKafkaConsumer(KafkaSettings settings, AffiliateCategoryDomainService domainService)
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
                ClientId = settings.ClientId,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        }

        public void Start(CancellationToken cancellationToken)
        {
            StartReceivingCreatedCategories(cancellationToken);
            StartReceivingChangedCategories(cancellationToken);
            StartReceivingCanceledCategories(cancellationToken);
        }

        private void StartReceivingCreatedCategories(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(CuponicoEvents.AffiliateCategoryCreated);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateCategoryCreated = JsonConvert.DeserializeObject<AffiliateCategoryCreated>(msg.Value);
                            _domainService.ProcessUnifiedCategory(affiliateCategoryCreated.Event).ConfigureAwait(false).GetAwaiter().GetResult();

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

        private void StartReceivingChangedCategories(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(CuponicoEvents.AffiliateCategoryChanged);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateCategoryChanged = JsonConvert.DeserializeObject<AffiliateCategoryChanged>(msg.Value);
                            _domainService.ProcessUnifiedCategory(affiliateCategoryChanged.Event).ConfigureAwait(false).GetAwaiter().GetResult();

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

        private void StartReceivingCanceledCategories(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<string, string>(_config).Build())
                {
                    consumer.Subscribe(CuponicoEvents.AffiliateCategoryCanceled);
                    while (true)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var msg = consumer.Consume(cancellationToken);

                            var affiliateCategoryCanceled = JsonConvert.DeserializeObject<AffiliateCategoryCanceled>(msg.Value);
                            _domainService.CancelUnifiedCategory(affiliateCategoryCanceled.Event).ConfigureAwait(false).GetAwaiter().GetResult();

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
