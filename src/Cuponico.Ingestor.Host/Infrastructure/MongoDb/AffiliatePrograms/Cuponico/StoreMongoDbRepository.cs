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
    public class StoreMongoDbRepository : IAffiliateStoreRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private string _collectinoName = "stores";

        public StoreMongoDbRepository(IMongoWrapper wrapper)
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

            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateStore>(_collectinoName);
        }

        public async Task<IList<AffiliateStore>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateStore>(_collectinoName);
        }

        public async Task SaveAsync(IList<AffiliateStore> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(_collectinoName, stores, x => y => x.StoreId == y.StoreId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateStore>.Filter;
                var filter = builder.Eq(c => c.StoreId, id);
                await Wrapper.DeleteOneAsync(_collectinoName, filter);
            }
        }
    }
}
