using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Lomadee;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Categories
{
    public class LomadeeCategoryMongoDbRepository
    {
        private readonly IMongoWrapper _wrapper;
        private string _collectinoName = "categories";

        public LomadeeCategoryMongoDbRepository(LomadeeMongoSettings mongoSettings)
        {
            if (mongoSettings == null)
                throw new ArgumentNullException(nameof(mongoSettings));

            _wrapper = mongoSettings.CreateWrapper();
            _wrapper.CreateCollectionIfNotExistsAsync<LomadeeCategory>(_collectinoName);
            _wrapper.CreateIndexIfNotExistsAsync<LomadeeCategory>(_collectinoName, "categoryId", null, e => e.Id);
        }

        public async Task<IList<LomadeeCategory>> GetAll()
        {
            return await _wrapper.FindAllAsync<LomadeeCategory>(_collectinoName);
        }

        public async Task SaveAsync(IList<LomadeeCategory> categories)
        {
            if (categories == null || !categories.Any()) return;
            await _wrapper.BulkWriteAsync(_collectinoName, categories, x => y => x.Id == y.Id);
        }

        public async Task DeleteAsync(IList<int> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<LomadeeCategory>.Filter;
                var filter = builder.Eq(c => c.Id, id);
                await _wrapper.DeleteOneAsync(_collectinoName, filter);
            }
        }
    }
}