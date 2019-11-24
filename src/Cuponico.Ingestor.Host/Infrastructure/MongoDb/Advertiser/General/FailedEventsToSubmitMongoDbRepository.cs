using System;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.General.Events;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Log;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.General
{
    public class FailedEventsToSubmitMongoDbRepository : IFailedEventRepository
    {
        private const string CollectinoName = "failed.events";
        protected readonly IMongoWrapper Wrapper;

        public FailedEventsToSubmitMongoDbRepository(LogMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();
            Wrapper.CreateCollectionIfNotExistsAsync<FailedEvent>(CollectinoName);

            if (!BsonClassMap.IsClassMapRegistered(typeof(FailedEvent)))
            {
                BsonClassMap.RegisterClassMap<FailedEvent>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.MapMember(c => c.Id).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }
        }

        public async Task SaveAsync(FailedEvent @event)
        {
            if (@event == null) return;
            await Wrapper.SaveAsync(CollectinoName, @event, x => x.Id == @event.Id);
        }
    }
}