using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories
{
    public interface IAffiliateCategoryRepository
    {
        Task<IList<AffiliateCategory>> GetAllAsync();
        Task SaveAsync(IList<AffiliateCategory> categories);
        Task DeleteAsync(IList<long> ids);
    }
}