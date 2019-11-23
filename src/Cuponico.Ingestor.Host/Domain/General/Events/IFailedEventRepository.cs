using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.General.Events
{
    public interface IFailedEventRepository
    {
        Task SaveAsync(FailedEvent @event);
    }
}