using System;
using Cuponico.Ingestor.Host.Domain;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Microsoft.Extensions.Configuration;

namespace Cuponico.Ingestor.Host.Infrastructure.Settings
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

            ClientId = kafkaSection.GetValue<string>(nameof(ClientId));
            GroupId = kafkaSection.GetValue<string>(nameof(GroupId));

            var topics = kafkaSection.GetSection("topics");

            CuponicoEvents.AffiliateStoreCreated = topics.GetValue<string>(nameof(AffiliateStoreCreated));
            CuponicoEvents.AffiliateStoreChanged = topics.GetValue<string>(nameof(AffiliateStoreChanged));
            CuponicoEvents.AffiliateStoreCanceled = topics.GetValue<string>(nameof(AffiliateStoreCanceled));
            CuponicoEvents.AffiliateCategoryCreated = topics.GetValue<string>(nameof(AffiliateCategoryCreated));
            CuponicoEvents.AffiliateCategoryChanged = topics.GetValue<string>(nameof(AffiliateCategoryChanged));
            CuponicoEvents.AffiliateCategoryCanceled = topics.GetValue<string>(nameof(AffiliateStoreCanceled));
        }

        public string BootstrapServers { get; }
        public string Username { get; }
        public string Password { get; }
        public string SchemaRegistryUrl { get; }
        public string SchemaRegistryUserName { get; }
        public string SchemaRegistryPassword { get; }
        public string ClientId { get; }
        public string GroupId { get; }
    }
}