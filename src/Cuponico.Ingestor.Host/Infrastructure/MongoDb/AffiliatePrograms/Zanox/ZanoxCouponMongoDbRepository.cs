﻿using Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Cuponico;

namespace Cuponico.Ingestor.Host.Infrastructure.MongoDb.AffiliatePrograms.Zanox
{
    public class ZanoxCouponMongoDbRepository : AffiliateCouponMongoDbRepository
    {
        public ZanoxCouponMongoDbRepository(ZanoxMongoSettings mongoSettings) : base(mongoSettings.CreateWrapper())
        {
        }
    }
}