using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser;
using Elevar.Infrastructure.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Advertiser;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico
{
    public class AffiliateCategoryMatchesMongoDbRepository : IAffiliateCategoryMatchesRepository
    {
        private const string CollectinoName = "affiliates.matched.categories";
        protected readonly IMongoWrapper Wrapper;

        public AffiliateCategoryMatchesMongoDbRepository(AdvertiserMongoSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            Wrapper = settings.CreateWrapper();

            if (!BsonClassMap.IsClassMapRegistered(typeof(AffiliateCategoryMatch)))
            {
                BsonClassMap.RegisterClassMap<AffiliateCategoryMatch>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.MapMember(c => c.Id).SetSerializer(new GuidSerializer(BsonType.String));
                    cm.MapMember(c => c.AdvertiseCategoryId).SetSerializer(new GuidSerializer(BsonType.String));
                });
            }

            Wrapper.CreateCollectionIfNotExistsAsync<AffiliateCategoryMatch>(CollectinoName);
            Wrapper.CreateIndexIfNotExistsAsync<AffiliateCategoryMatch>(CollectinoName, "AdvertiseCategoryId", 
                null,
                advertiseCategoryId => advertiseCategoryId.AdvertiseCategoryId, 
                program => program.AffiliateProgram,
                categoryId => categoryId.AffiliateCategoryId);
        }

        public async Task DeleteAsync(IList<Guid> ids)
        {
            foreach (var id in ids)
            {
                var builder = Builders<AffiliateCategoryMatch>.Filter;
                var filter = builder.Eq(c => c.AdvertiseCategoryId, id);
                await Wrapper.DeleteOneAsync(CollectinoName, filter);
            }
        }

        public async Task<IList<AffiliateCategoryMatch>> GetAllAsync()
        {
            return await Wrapper.FindAllAsync<AffiliateCategoryMatch>(CollectinoName);
        }

        public async Task SaveAsync(AffiliateCategoryMatch matchedCategory)
        {
            if (matchedCategory == null) return;
            await Wrapper.SaveAsync(CollectinoName, matchedCategory, x => x.AdvertiseCategoryId == matchedCategory.AdvertiseCategoryId 
                                                                       && x.AffiliateProgram == matchedCategory.AffiliateProgram 
                                                                       && x.AffiliateCategoryId == matchedCategory.AffiliateCategoryId);
        }
    }
}