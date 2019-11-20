using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public interface IAffiliateStoreRepository
    {
        Task<IList<AffiliateStore>> GetAllAsync();
        Task SaveAsync(IList<AffiliateStore> stores);
        Task DeleteAsync(IList<long> ids);
    }
}
