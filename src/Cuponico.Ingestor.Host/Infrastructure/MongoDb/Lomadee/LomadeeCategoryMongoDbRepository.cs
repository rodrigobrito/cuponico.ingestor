using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Lomadee
{
    public class LomadeeCategoryMongoDbRepository : CategoryMongoDbRepository
    {
        public LomadeeCategoryMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}