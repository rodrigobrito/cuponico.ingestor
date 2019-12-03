using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateStoreMatchesMongoDbRepository : IAffiliateStoreMatchesRepository
    {
        private const string CollectinoName = "affiliates.matched.stores";
        protected readonly IMongoWrapper Wrapper;

        public AffiliateStoreMatchesMongoDbRepository(AdvertiserMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();
            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateStoreMatch>(CollectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<AffiliateStoreMatch>(CollectinoName, "AdvertiseStoreId", 
                null,
                advertiseStoreId => advertiseStoreId.AdvertiseStoreId, 
                program => program.AffiliateProgram,
                storeId => storeId.AffiliateStoreId);
        }

        public async Task DeleteAsync(IList<Guid> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateStoreMatch>.Filter;
                var filter = builder.Eq(c => c.AdvertiseStoreId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }

        public async Task<IList<AffiliateStoreMatch>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateStoreMatch>(CollectinoName);
        }

        public async Task SaveAsync(AffiliateStoreMatch matchedStore)
        {
            if (matchedStore == null) return;
            await Wrapper.SaveAsync(CollectinoName, matchedStore, x => x.AdvertiseStoreId == matchedStore.AdvertiseStoreId 
                                                                       && x.AffiliateProgram == matchedStore.AffiliateProgram 
                                                                       && x.AffiliateStoreId == matchedStore.AffiliateStoreId);
        }

        public static void RegisterClassMap()
        {
            BsonClassMap.RegisterClassMap<AffiliateStoreMatch>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
                cm.MapMember(c => c.Id).SetSerializer(new GuidSerializer(BsonType.String));
                cm.MapMember(c => c.AdvertiseStoreId).SetSerializer(new GuidSerializer(BsonType.String));
            });
        }
    }
}