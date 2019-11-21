using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox
{
    public class ZanoxStoreMongoDbRepository : AffiliateStoreMongoDbRepository
    {
        public ZanoxStoreMongoDbRepository(ZanoxMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}