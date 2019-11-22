using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategoryCreated : DomainEvent<long, AffiliateCategory>
    {
        public const string AffiliateEventName = "affiliate.category.created";
        public AffiliateCategoryCreated()
        {
        }

        protected AffiliateCategoryCreated(long id, AffiliateCategory @event, DateTime createdDate) : base(id, @event, AffiliateEventName, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateCategory>> CreateMany(IList<AffiliateCategory> categories)
        {
            var events = new List<DomainEvent<long, AffiliateCategory>>();
            foreach (var category in categories)
            {
                var storeCreated = Create(category.CategoryId, category, AffiliateEventName, DateTime.Now);
                events.Add(storeCreated);
            }
            return events;
        }
    }
}