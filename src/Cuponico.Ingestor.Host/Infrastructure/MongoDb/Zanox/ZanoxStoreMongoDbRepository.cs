using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Zanox
{
    public class ZanoxStoreMongoDbRepository : StoreMongoDbRepository
    {
        public ZanoxStoreMongoDbRepository(ZanoxMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}