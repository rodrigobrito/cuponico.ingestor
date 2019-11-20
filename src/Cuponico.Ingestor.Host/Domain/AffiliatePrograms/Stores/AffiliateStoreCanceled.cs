using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public class AffiliateStoreCanceled : DomainEvent<long, AffiliateStore>
    {
        protected AffiliateStoreCanceled(long id, AffiliateStore @event, DateTime createdDate) : base(id, @event, "store.canceled", createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateStore>> CreateMany(IList<AffiliateStore> stores)
        {
            var events = new List<DomainEvent<long, AffiliateStore>>();
            foreach (var store in stores)
            {
                var storeCreated = AffiliateStoreCreated.Create(store.StoreId, store, "store.canceled", DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}
