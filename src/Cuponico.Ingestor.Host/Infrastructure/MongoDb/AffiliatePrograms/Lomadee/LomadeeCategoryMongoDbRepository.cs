using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Lomadee;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee
{
    public class LomadeeCategoryMongoDbRepository : AffiliateCategoryMongoDbRepository
    {
        public LomadeeCategoryMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}