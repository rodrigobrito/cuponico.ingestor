using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cuponico.Ingestor.Host.Domain.Advertiser.Categories
{
    public interface ICategoryRepository
    {
        Task<IList<Category>> GetAllAsync();
        Task SaveAsync(Category category);
        Task SaveAsync(IList<Category> categories);
        Task DeleteAsync(IList<Guid> ids);
    }
}