using System;
using System.Collections.Generic;

namespace Cuponico.Ingestor.Host.Domain
{
    public class DomainEvent<TK, T> where TK : struct
    {
        public TK Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public T Event { get; set; }
        public string EventName { get; set; }
        protected DomainEvent() { }

        protected DomainEvent(TK id, T @event, string eventName, DateTime createdDate)
        {
            Id = id;
            Event = @event;
            EventName = eventName;
            CreatedDate = createdDate;
        }

        public static DomainEvent<TK, T> Create(TK id, T @event, string eventName, DateTime? createdDate)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw  new ArgumentNullException(nameof(eventName));

            if (@event == null)
                throw  new ArgumentNullException(nameof(@event));

            return new DomainEvent<TK, T>(id, @event, eventName, createdDate ?? DateTime.UtcNow);
        }

        public static IList<DomainEvent<TK, T>> CreateMany(IList<TK> ids, IList<T> @events, string eventName, DateTime? createdDate = null)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (@events == null)
                throw new ArgumentNullException(nameof(@events));

            if (ids.Count != @events.Count)
                throw new ArgumentOutOfRangeException(nameof(events), "Event size is different of ids size.");

            var createdEvents = new List<DomainEvent<TK, T>>();
            for (var i = 0; i < ids.Count; i++)
            {
                var id = ids[i];
                var ev = @events[i];
                var createdEvent = Create(id, ev, eventName, createdDate);
                createdEvents.Add(createdEvent);
            }
            return createdEvents;
        }
    }
}
