﻿using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public class AffiliateStoreCanceled : DomainEvent<long, AffiliateStore>
    {
        public AffiliateStoreCanceled()
        {
        }

        protected AffiliateStoreCanceled(long id, AffiliateStore @event, DateTime createdDate) : base(id, @event, CuponicoEvents.AffiliateStoreCanceled, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateStore>> CreateMany(IList<AffiliateStore> stores)
        {
            var events = new List<DomainEvent<long, AffiliateStore>>();
            foreach (var store in stores)
            {
                var storeCreated = Create(store.StoreId, store, CuponicoEvents.AffiliateStoreCanceled, DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}
