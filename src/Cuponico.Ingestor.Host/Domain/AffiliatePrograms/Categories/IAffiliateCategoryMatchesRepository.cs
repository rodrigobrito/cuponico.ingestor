using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public interface IAffiliateCategoryMatchesRepository
    {
        Task<IList<AffiliateCategoryMatch>> GetAllAsync();
        Task SaveAsync(AffiliateCategoryMatch matchedCategory);
        Task DeleteAsync(IList<Guid> ids);
    }
}