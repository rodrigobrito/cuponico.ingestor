using System;
using Microsoft.Extensions.Configuration;

namespace Cuponico.Ingestor.Host.Kafka
{
    public class KafkaSettings
    {
        public KafkaSettings(IConfigurationRoot section)
        {
            if (section == null) 
                throw new ArgumentNullException(nameof(section));

            var kafkaSection = section.GetSection(nameof(Kafka));
            var cluster = kafkaSection.GetSection("Cluster");
            
            BootstrapServers = cluster.GetValue<string>(nameof(BootstrapServers));
            Username = cluster.GetValue<string>(nameof(Username));
            Password = cluster.GetValue<string>(nameof(Password));

            var schemaRegistry = kafkaSection.GetSection("SchemaRegistry");
            SchemaRegistryUrl = schemaRegistry.GetValue<string>("Url");
            SchemaRegistryUserName = schemaRegistry.GetValue<string>("Username");
            SchemaRegistryPassword = schemaRegistry.GetValue<string>("Password");
        }

        public string BootstrapServers { get; }
        public string Username { get; }
        public string Password { get; }
        public string SchemaRegistryUrl { get;  }
        public string SchemaRegistryUserName { get; }
        public string SchemaRegistryPassword { get; }
    }
}