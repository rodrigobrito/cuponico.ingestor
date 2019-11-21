using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public class AffiliateStoreCreated : DomainEvent<long, AffiliateStore>
    {
        public const string AffiliateEventName = "affiliate.store.created";

        public AffiliateStoreCreated()
        {
        }

        protected AffiliateStoreCreated(long id, AffiliateStore @event, DateTime createdDate) : base(id, @event, AffiliateEventName, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateStore>> CreateMany(IList<AffiliateStore> stores)
        {
            var events = new List<DomainEvent<long, AffiliateStore>>();
            foreach (var store in stores)
            {
                var storeCreated = Create(store.StoreId, store, AffiliateEventName, DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}