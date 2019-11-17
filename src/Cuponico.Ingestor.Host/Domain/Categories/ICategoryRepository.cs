using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Categories
{
    public interface ICategoryRepository
    {
        Task<IList<Category>> GetAllAsync();
        Task SaveAsync(IList<Category> categories);
        Task DeleteAsync(IList<long> ids);
    }
}