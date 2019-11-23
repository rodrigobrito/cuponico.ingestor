using System;

namespace Cuponico.Ingestor.Host.Domain.General.Events
{
    public class FailedEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}