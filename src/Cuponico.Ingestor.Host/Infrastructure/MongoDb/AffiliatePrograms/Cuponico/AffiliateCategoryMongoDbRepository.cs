using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateCategoryMongoDbRepository : IAffiliateCategoryRepository
    {
        private const string CollectinoName = "affiliate.categories";
        protected readonly IMongoWrapper Wrapper;

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

            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateStore>(CollectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<AffiliateStore>(CollectinoName, "categoryId", null, e => e.StoreId);
        }

        public async Task<IList<AffiliateCategory>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateCategory>(CollectinoName);
        }

        public async Task SaveAsync(IList<AffiliateCategory> stores)
        {
            if (stores == null || !stores.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, stores, x => y => x.CategoryId == y.CategoryId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateCategory>.Filter;
                var filter = builder.Eq(c => c.CategoryId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }
    }
}
