using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategoryChanged : DomainEvent<long, AffiliateCategory>
    {
        public AffiliateCategoryChanged()
        {
        }
        protected AffiliateCategoryChanged(long id, AffiliateCategory @event, DateTime createdDate) : base(id, @event, CuponicoEvents.AffiliateCategoryChanged, createdDate)
        {
        }
        public static IList<DomainEvent<long, AffiliateCategory>> CreateMany(IList<AffiliateCategory> categories)
        {
            var events = new List<DomainEvent<long, AffiliateCategory>>();
            foreach (var category in categories)
            {
                var categoryChanged = Create(category.CategoryId, category, CuponicoEvents.AffiliateCategoryChanged, DateTime.Now);
                events.Add(categoryChanged);
            }
            return events;
        }
    }
}