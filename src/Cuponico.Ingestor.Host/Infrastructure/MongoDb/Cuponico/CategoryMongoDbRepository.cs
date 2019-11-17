using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Categories;
using Cuponico.Ingestor.Host.Domain.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico
{
    public class CategoryMongoDbRepository : ICategoryRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private string _collectinoName = "categories";

        public CategoryMongoDbRepository(IMongoWrapper wrapper)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Category)))
            {
                BsonClassMap.RegisterClassMap<Category>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.CategoryId);
                });
            }

            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));

            Wrapper.CreateCollectionIfNotExistsAsync<Store>(_collectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<Store>(_collectinoName, "categoryId", null, e => e.StoreId);
        }

        public async Task<IList<Category>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<Category>(_collectinoName);
        }

        public async Task SaveAsync(IList<Category> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(_collectinoName, stores, x => y => x.CategoryId == y.CategoryId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<Category>.Filter;
                var filter = builder.Eq(c => c.CategoryId, id);
                await Wrapper.DeleteOneAsync(_collectinoName, filter);
            }
        }
    }
}
