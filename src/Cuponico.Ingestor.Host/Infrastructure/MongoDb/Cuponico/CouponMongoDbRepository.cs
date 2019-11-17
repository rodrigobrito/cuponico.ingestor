using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Http.Lomadee.Coupons.Tickets;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico
{
    public class CouponMongoDbRepository : ICouponRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private const string CollectinoName = "coupons";

        public CouponMongoDbRepository(IMongoWrapper wrapper)
        {
            BsonClassMap.RegisterClassMap<Coupon>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });

            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
            Wrapper.CreateCollectionIfNotExistsAsync<LomadeeCoupon>(CollectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<LomadeeCoupon>(CollectinoName, "couponId", null, e => e.Id);
        }

        public async Task<IList<Coupon>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<Coupon>(CollectinoName);
        }

        public async Task SaveAsync(IList<Coupon> coupons)
        {
            if (coupons == null || !coupons.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, coupons, x => y => x.Id == y.Id);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<Coupon>.Filter;
                var filter = builder.Eq(c => c.Id, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
            throw new NotImplementedException();
        }
    }
}
