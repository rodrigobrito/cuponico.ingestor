using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee
{
    public class LomadeeCouponMongoDbRepository : AffiliateCouponMongoDbRepository
    {
        public LomadeeCouponMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}