using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public class AffiliateCategoryCanceled : DomainEvent<long, AffiliateCategory>
    {
        public const string AffiliateEventName = "affiliate.category.canceled";
        public AffiliateCategoryCanceled()
        {
        }

        protected AffiliateCategoryCanceled(long id, AffiliateCategory @event, DateTime createdDate) : base(id, @event, AffiliateEventName, createdDate)
        {
        }

        public static IList<DomainEvent<long, AffiliateCategory>> CreateMany(IList<AffiliateCategory> categories)
        {
            var events = new List<DomainEvent<long, AffiliateCategory>>();
            foreach (var category in categories)
            {
                var categoryCreated = Create(category.CategoryId, category, AffiliateEventName, DateTime.Now);
                events.Add(categoryCreated);
            }
            return events;
        }
    }
}