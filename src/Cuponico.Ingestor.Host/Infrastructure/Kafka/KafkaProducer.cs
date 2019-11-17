using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace Cuponico.Ingestor.Host.Infrastructure.Kafka
{
    public class KafkaProducer<TK, T>
    {
        private readonly IProducer<TK, T> _producer;

        public KafkaProducer(KafkaSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));


            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                Acks = Acks.All,
                MessageSendMaxRetries = 10000000,
                ApiVersionRequest = true,
                BrokerVersionFallback = "0.10.0.0",
                ApiVersionFallbackMs = 0,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                //SslCaLocation = "/usr/local/etc/openssl/cert.pem",
                SaslUsername = settings.Username,
                SaslPassword = settings.Password
            };

            var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                SchemaRegistryUrl = settings.SchemaRegistryUrl,
                SchemaRegistryBasicAuthUserInfo = $"{settings.SchemaRegistryUserName}:{settings.SchemaRegistryPassword}"
            });

            _producer = new ProducerBuilder<TK, T>(config)
                .SetKeySerializer(new AvroSerializer<TK>(schemaRegistry))
                .SetValueSerializer(new AvroSerializer<T>(schemaRegistry))
                .Build();
        }

        public async Task<DeliveryResult<TK, T>> SendAsync(string topic, TK key, T message)
        {
            var msg = new Message<TK, T>
            {
                Key = key,
                Value = message
            };
            return await _producer.ProduceAsync(topic, msg);
        }


        public void Send(string topic, TK key, T message)
        {
            var msg = new Message<TK, T>
            {
                Key = key,
                Value = message
            };
            _producer.Produce(topic, msg);
        }

        public void Send(string topic, IList<KeyValuePair<TK, T>> messages, Action<DeliveryReport<TK, T>> delivered = null)
        {
            if (messages == null || !messages.Any())
                throw new InvalidEnumArgumentException($"Expected at least one message.");

            if (messages.Count > 1000)
                throw new InvalidEnumArgumentException($"A maximum of 1000 messages is allowed.");

            foreach (var message in messages)
            {
                var msg = new Message<TK, T>
                {
                    Key = message.Key,
                    Value = message.Value
                };
                _producer.Produce(topic, msg, delivered);
            }
            _producer.Flush(TimeSpan.FromSeconds(60));          // wait for up to 60 seconds for any inflight messages to be delivered.
        }
    }
}
