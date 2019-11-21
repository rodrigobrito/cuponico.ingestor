using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateCouponMongoDbRepository : IAffiliateCouponRepository
    {
        protected readonly IMongoWrapper Wrapper;
        private const string CollectinoName = "coupons";

        public AffiliateCouponMongoDbRepository(IMongoWrapper wrapper)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(AffiliateCoupon)))
            {
                BsonClassMap.RegisterClassMap<AffiliateCoupon>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.CouponId);
                });
            }

            Wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateCoupon>(CollectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<AffiliateCoupon>(CollectinoName, "couponId", null, e => e.CouponId);
        }

        public async Task<IList<AffiliateCoupon>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateCoupon>(CollectinoName);
        }

        public async Task SaveAsync(IList<AffiliateCoupon> coupons)
        {
            if (coupons == null || !coupons.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, coupons, x => y => x.CouponId == y.CouponId);
        }

        public async Task DeleteAsync(IList<long> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateCoupon>.Filter;
                var filter = builder.Eq(c => c.CouponId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
            throw new NotImplementedException();
        }
    }
}
