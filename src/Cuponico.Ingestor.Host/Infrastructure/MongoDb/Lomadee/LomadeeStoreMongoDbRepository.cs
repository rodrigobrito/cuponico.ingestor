using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Lomadee
{
    public class LomadeeStoreMongoDbRepository : StoreMongoDbRepository
    {
        public LomadeeStoreMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}