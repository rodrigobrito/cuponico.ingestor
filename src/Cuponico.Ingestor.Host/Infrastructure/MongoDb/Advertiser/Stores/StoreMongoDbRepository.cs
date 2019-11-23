using Cuponico.Ingestor.Host.Domain.Advertiser.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Stores
{
    public class StoreMongoDbRepository: IStoreRepository
    {
        private const string CollectinoName = "stores";
        protected readonly IMongoWrapper Wrapper;

        public StoreMongoDbRepository(AdvertiserMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();
            Wrapper.CreateCollectionIfNotExistsAsync<Store>(CollectinoName);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Store)))
            {
                BsonClassMap.RegisterClassMap<Store>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.StoreId);
                    cm.MapMember(c => c.StoreId).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }
        }

        public async Task<IList<Store>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<Store>(CollectinoName);
        }

        public async Task SaveAsync(Store store)
        {
            if (store == null) return;
            await Wrapper.SaveAsync(CollectinoName, store, x => x.StoreId == store.StoreId);
        }

        public async Task SaveAsync(IList<Store> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, stores, x => y => x.StoreId == y.StoreId);
        }

        public async Task DeleteAsync(IList<Guid> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<Store>.Filter;
                var filter = builder.Eq(c => c.StoreId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }
    }
}
