using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox
{
    public class ZanoxCategoryMongoDbRepository: AffiliateCategoryMongoDbRepository
    {
        public ZanoxCategoryMongoDbRepository(ZanoxMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}