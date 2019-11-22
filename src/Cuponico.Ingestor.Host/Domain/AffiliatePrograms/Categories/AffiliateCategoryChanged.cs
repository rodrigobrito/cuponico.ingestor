using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategoryChanged : DomainEvent<long, AffiliateCategory>
    {
        public const string AffiliateEventName = "affiliate.category.changed";
        public AffiliateCategoryChanged()
        {
        }
        protected AffiliateCategoryChanged(long id, AffiliateCategory @event, DateTime createdDate) : base(id, @event, AffiliateEventName, createdDate)
        {
        }
        public static IList<DomainEvent<long, AffiliateCategory>> CreateMany(IList<AffiliateCategory> categories)
        {
            var events = new List<DomainEvent<long, AffiliateCategory>>();
            foreach (var category in categories)
            {
                var categoryChanged = Create(category.CategoryId, category, AffiliateEventName, DateTime.Now);
                events.Add(categoryChanged);
            }
            return events;
        }
    }
}