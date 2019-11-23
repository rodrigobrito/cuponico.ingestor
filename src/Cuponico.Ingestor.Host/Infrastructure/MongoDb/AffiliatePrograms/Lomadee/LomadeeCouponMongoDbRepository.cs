using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;
using Cuponico.Ingestor.Host.Infrastructure.Settings.Lomadee;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Lomadee
{
    public class LomadeeCouponMongoDbRepository : AffiliateCouponMongoDbRepository
    {
        public LomadeeCouponMongoDbRepository(LomadeeMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}