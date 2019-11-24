using Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Domain.Advertiser.Coupons;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Coupons
{
    public class CouponMongoDbRepository: ICouponRepository
    {
        private const string CollectinoName = "coupons";
        protected readonly IMongoWrapper Wrapper;

        public CouponMongoDbRepository(AdvertiserMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();
            Wrapper.CreateCollectionIfNotExistsAsync<Coupon>(CollectinoName);

            if (!BsonClassMap.IsClassMapRegistered(typeof(Coupon)))
            {
                BsonClassMap.RegisterClassMap<Coupon>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.CouponId);
                    cm.MapMember(c => c.CouponId).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }
        }

        public async Task<IList<Coupon>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<Coupon>(CollectinoName);
        }

        public async Task SaveAsync(Coupon coupon)
        {
            if (coupon == null) return;
            await Wrapper.SaveAsync(CollectinoName, coupon, x => x.CouponId == coupon.CouponId);
        }

        public async Task SaveAsync(IList<Coupon> coupons)
        {
            if (coupons == null || !coupons.Any()) return;
            await Wrapper.BulkWriteAsync(CollectinoName, coupons, x => y => x.CouponId == y.CouponId);
        }

        public async Task DeleteAsync(IList<Guid> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<Coupon>.Filter;
                var filter = builder.Eq(c => c.CouponId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }
    }
}
