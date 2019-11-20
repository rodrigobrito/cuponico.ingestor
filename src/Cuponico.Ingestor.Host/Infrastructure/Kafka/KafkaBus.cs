using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Cuponico.Ingestor.Host.Domain;
using Newtonsoft.Json;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka
{
    public class KafkaBus: IPublisher
    {
        private readonly IProducer<string, string> _producer;

        public KafkaBus(KafkaSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                Acks = Acks.All,
                MessageSendMaxRetries = 10000000,
                SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                //SslCaLocation = "/usr/local/etc/openssl/cert.pem",
                SaslUsername = settings.Username,
                SaslPassword = settings.Password
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<TK, T>(DomainEvent<TK, T> domainEvent) where TK: struct
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var data = JsonConvert.SerializeObject(domainEvent, Formatting.Indented);
            var msg = new Message<string, string>
            {
                Key = domainEvent.Id.ToString(),
                Value = data
            };
            await _producer.ProduceAsync(domainEvent.EventName, msg);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task PublishAsync<TK, T>(IList<DomainEvent<TK, T>> domainEvents) where TK : struct
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (domainEvents == null || !domainEvents.Any())
                throw new ArgumentNullException(nameof(domainEvents));

            foreach (var domainEvent in domainEvents)
            {
                var data = JsonConvert.SerializeObject(domainEvent, Formatting.Indented);
                var msg = new Message<string, string>
                {
                    Key = domainEvent.Id.ToString(),
                    Value = data
                };
                _producer.Produce(domainEvent.EventName, msg, (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                    }
                });
            }
            _producer.Flush(TimeSpan.FromSeconds(10)); // wait for up to 10 seconds for any inflight messages to be delivered.
        }
    }
}