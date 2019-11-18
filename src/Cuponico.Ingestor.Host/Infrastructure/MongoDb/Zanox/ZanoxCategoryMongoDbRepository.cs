using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Zanox
{
    public class ZanoxCategoryMongoDbRepository: CategoryMongoDbRepository
    {
        public ZanoxCategoryMongoDbRepository(ZanoxMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}