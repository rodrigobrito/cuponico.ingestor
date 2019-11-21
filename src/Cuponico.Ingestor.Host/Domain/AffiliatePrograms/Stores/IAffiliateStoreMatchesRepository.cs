using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores
{
    public interface IAffiliateStoreMatchesRepository
    {
        Task<IList<AffiliateStoreMatch>> GetAllAsync();
        Task SaveAsync(AffiliateStoreMatch matchedStore);
        Task DeleteAsync(IList<Guid> ids);
    }
}