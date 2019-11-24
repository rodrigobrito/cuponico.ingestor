using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Advertiser.Categories;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Categories
{
    public class CategoryMongoDbRepository : ICategoryRepository
    {
        private const string CollectinoName = "categories";
        protected readonly IMongoWrapper Wrapper;

        public CategoryMongoDbRepository(AdvertiserMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();
            Wrapper.CreateCollectionIfNotExistsAsync<Category>(CollectinoName);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Category)))
            {
                BsonClassMap.RegisterClassMap<Category>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.CategoryId);
                    cm.MapMember(c => c.CategoryId).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }

            SaveAsync(Category.GetDefaultCategory()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<IList<Category>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<Category>(CollectinoName);
        }

        public async Task SaveAsync(Category category)
        {
            if (category == null) return;
            await Wrapper.SaveAsync(CollectinoName, category, x => x.CategoryId == category.CategoryId);
        }

        public async Task SaveAsync(IList<Category> categories)
        {
            if (categories == null || !categories.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, categories, x => y => x.CategoryId == y.CategoryId);
        }

        public async Task DeleteAsync(IList<Guid> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<Category>.Filter;
                var filter = builder.Eq(c => c.CategoryId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }
    }
}
