using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico
{
    public class StoreMongoDbRepository : IStoreRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private string _collectinoName = "stores";

        public StoreMongoDbRepository(IMongoWrapper wrapper)
        {
            BsonClassMap.RegisterClassMap<Store>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.StoreId);
            });

            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));

            Wrapper.CreateCollectionIfNotExistsAsync<Store>(_collectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<Store>(_collectinoName, "storeId", null, e => e.StoreId);
        }

        public async Task<IList<Store>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<Store>(_collectinoName);
        }

        public async Task SaveAsync(IList<Store> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(_collectinoName, stores, x => y => x.StoreId == y.StoreId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<Store>.Filter;
                var filter = builder.Eq(c => c.StoreId, id);
                await Wrapper.DeleteOneAsync(_collectinoName, filter);
            }
        }
    }
}
