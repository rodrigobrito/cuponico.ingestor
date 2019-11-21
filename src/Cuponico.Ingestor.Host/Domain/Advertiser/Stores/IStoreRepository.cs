using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Advertiser.Stores
{
    public interface IStoreRepository
    {
        Task<IList<Store>> GetAllAsync();
        Task SaveAsync(Store store);
        Task SaveAsync(IList<Store> stores);
        Task DeleteAsync(IList<Guid> ids);
    }
}
