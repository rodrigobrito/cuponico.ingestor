using Elevar.Infrastructure.MongoDb;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ingestor.ConsoleHost.Partners.Lomadee.Coupons.Tickets
{
    public class LomadeeCouponMongoDbRepository
    {
        private readonly IMongoWrapper _wrapper;
        private string _collectinoName = "coupons";

        public LomadeeCouponMongoDbRepository(LomadeeMongoSettings mongoSettings)
        {
            if (mongoSettings == null)
                throw new ArgumentNullException(nameof(mongoSettings));

            _wrapper = mongoSettings.CreateWrapper();
            _wrapper.CreateCollectionIfNotExistsAsync<LomadeeStore>(_collectinoName);
            _wrapper.CreateIndexIfNotExistsAsync<LomadeeStore>(_collectinoName, "couponId", null, e => e.Id);
        }

        public async Task<IList<LomadeeCoupon>> GetAll()
        {
            return await _wrapper.FindAllAsync<LomadeeCoupon>(_collectinoName);
        }

        public async Task SaveAsync(IList<LomadeeCoupon> coupons)
        {
            if (coupons == null || !coupons.Any()) return;
            await _wrapper.BulkWriteAsync(_collectinoName, coupons, x => y => x.Id == y.Id);
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
