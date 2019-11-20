using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain
{
    public interface IPublisher
    {
        Task PublishAsync<TK, T>(DomainEvent<TK, T> domainEvent) where TK: struct;
        Task PublishAsync<TK, T>(IList<DomainEvent<TK, T>> domainEvents) where TK: struct;
    }
}