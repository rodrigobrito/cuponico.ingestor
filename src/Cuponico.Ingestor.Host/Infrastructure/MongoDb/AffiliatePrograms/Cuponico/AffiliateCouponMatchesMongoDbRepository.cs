using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateCouponMatchesMongoDbRepository : IAffiliateCouponMatchesRepository
    {
        private const string CollectinoName = "affiliates.matched.coupons";
        protected readonly IMongoWrapper Wrapper;

        public AffiliateCouponMatchesMongoDbRepository(AdvertiserMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();

            if (!BsonClassMap.IsClassMapRegistered(typeof(AffiliateCouponMatch)))
            {
                BsonClassMap.RegisterClassMap<AffiliateCouponMatch>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.MapMember(c => c.Id).SetSerializer(new GuidSerializer(BsonType.String));
                    cm.MapMember(c => c.AdvertiseCouponId).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }

            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateCouponMatch>(CollectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<AffiliateCouponMatch>(CollectinoName, "AdvertiseCouponId", 
                null,
                advertiseCouponId => advertiseCouponId.AdvertiseCouponId, 
                program => program.AffiliateProgram,
                couponId => couponId.AffiliateCouponId);
        }

        public async Task DeleteAsync(IList<Guid> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateCouponMatch>.Filter;
                var filter = builder.Eq(c => c.AdvertiseCouponId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }

        public async Task<IList<AffiliateCouponMatch>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateCouponMatch>(CollectinoName);
        }

        public async Task SaveAsync(AffiliateCouponMatch matchedCoupon)
        {
            if (matchedCoupon == null) return;
            await Wrapper.SaveAsync(CollectinoName, matchedCoupon, x => x.AdvertiseCouponId == matchedCoupon.AdvertiseCouponId 
                                                                       && x.AffiliateProgram == matchedCoupon.AffiliateProgram 
                                                                       && x.AffiliateCouponId == matchedCoupon.AffiliateCouponId);
        }
    }
}