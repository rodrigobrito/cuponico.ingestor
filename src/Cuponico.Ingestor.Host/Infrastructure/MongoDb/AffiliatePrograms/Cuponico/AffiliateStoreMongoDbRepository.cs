using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateStoreMongoDbRepository : IAffiliateStoreRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private const string CollectinoName = "affiliate.stores";

        public AffiliateStoreMongoDbRepository(IMongoWrapper wrapper)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(AffiliateStore)))
            {
                BsonClassMap.RegisterClassMap<AffiliateStore>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.StoreId);
                });
            }

            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));

            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateStore>(CollectinoName);
        }

        public async Task<IList<AffiliateStore>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateStore>(CollectinoName);
        }

        public async Task SaveAsync(IList<AffiliateStore> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, stores, x => y => x.StoreId == y.StoreId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateStore>.Filter;
                var filter = builder.Eq(c => c.StoreId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }
    }
}
