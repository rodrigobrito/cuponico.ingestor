using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public class AffiliateStoreChanged : DomainEvent<long, AffiliateStore>
    {
        protected AffiliateStoreChanged(long id, AffiliateStore @event, DateTime createdDate) : base(id, @event, "store.changed", createdDate)
        {
        }
        public static IList<DomainEvent<long, AffiliateStore>> CreateMany(IList<AffiliateStore> stores)
        {
            var events = new List<DomainEvent<long, AffiliateStore>>();
            foreach (var store in stores)
            {
                var storeCreated = AffiliateStoreCreated.Create(store.StoreId, store, "store.changed", DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}
