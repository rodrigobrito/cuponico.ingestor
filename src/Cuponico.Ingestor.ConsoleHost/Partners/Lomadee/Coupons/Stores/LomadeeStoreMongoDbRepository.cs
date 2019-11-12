using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Partners.Lomadee.Coupons.Stores
{
    public class LomadeeStoreMongoDbRepository
    {
        private readonly IMongoWrapper _wrapper;
        private string _collectinoName = "stores";

        public LomadeeStoreMongoDbRepository(LomadeeMongoSettings mongoSettings)
        {
            if (mongoSettings == null)
                throw new ArgumentNullException(nameof(mongoSettings));

            _wrapper = mongoSettings.CreateWrapper();
            _wrapper.CreateCollectionIfNotExistsAsync<LomadeeStore>(_collectinoName);
            _wrapper.CreateIndexIfNotExistsAsync<LomadeeStore>(_collectinoName, "storeId", null, e => e.Id);
        }

        public async Task<IList<LomadeeStore>> GetAll()
        {
            return await _wrapper.FindAllAsync<LomadeeStore>(_collectinoName);
        }

        public async Task SaveAsync(IList<LomadeeStore> stores)
        {
            if (stores == null || !stores.Any()) return;
            await _wrapper.BulkWriteAsync(_collectinoName, stores, x => y => x.Id == y.Id);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<LomadeeStore>.Filter;
                var filter = builder.Eq(c => c.Id, id);
                await _wrapper.DeleteOneAsync(_collectinoName, filter);
            }
        }
    }
}