using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Categories;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Coupons;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Advertiser.Stores;
using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;
using MongoDB.Bson.Serialization.Conventions;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb
{
    public class MongoRegister
    {
        public static void RegisterClassMap()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
            };
            ConventionRegistry.Register("camel case", pack, t => true);

            CouponMongoDbRepository.RegisterClassMap();
            CategoryMongoDbRepository.RegisterClassMap();
            StoreMongoDbRepository.RegisterClassMap();

            AffiliateCategoryMongoDbRepository.RegisterClassMap();
            AffiliateCategoryMatchesMongoDbRepository.RegisterClassMap();
            AffiliateStoreMongoDbRepository.RegisterClassMap();
            AffiliateStoreMatchesMongoDbRepository.RegisterClassMap();
            AffiliateCouponMongoDbRepository.RegisterClassMap();
            AffiliateCouponMatchesMongoDbRepository.RegisterClassMap();
        }
    }
}
