using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee
{
    public class LomadeeCategoryMongoDbRepository : AffiliateCategoryMongoDbRepository
    {
        public LomadeeCategoryMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}