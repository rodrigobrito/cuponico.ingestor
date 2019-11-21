using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateCategoryMongoDbRepository : IAffiliateCategoryRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private string _collectinoName = "categories";

        public AffiliateCategoryMongoDbRepository(IMongoWrapper wrapper)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(AffiliateCategory)))
            {
                BsonClassMap.RegisterClassMap<AffiliateCategory>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.CategoryId);
                });
            }

            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));

            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateStore>(_collectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<AffiliateStore>(_collectinoName, "categoryId", null, e => e.StoreId);
        }

        public async Task<IList<AffiliateCategory>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateCategory>(_collectinoName);
        }

        public async Task SaveAsync(IList<AffiliateCategory> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(_collectinoName, stores, x => y => x.CategoryId == y.CategoryId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateCategory>.Filter;
                var filter = builder.Eq(c => c.CategoryId, id);
                await Wrapper.DeleteOneAsync(_collectinoName, filter);
            }
        }
    }
}
