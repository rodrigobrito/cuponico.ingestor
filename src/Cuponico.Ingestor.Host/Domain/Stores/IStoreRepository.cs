using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Stores
{
    public interface IStoreRepository
    {
        Task<IList<Store>> GetAllAsync();
        Task SaveAsync(IList<Store> stores);
        Task DeleteAsync(IList<long> ids);
    }
}
