using Cuponico.Ingestor.Host.Infrastructure.MongoDb.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.Lomadee
{
    public class LomadeeCouponMongoDbRepository : CouponMongoDbRepository
    {
        public LomadeeCouponMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}