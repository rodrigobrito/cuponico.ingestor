using Cuponico.Ingestor.Host.Domain.Categories;

namespace Cuponico.Ingestor.Host.Domain.Jobs
{
    public class CategoriesSchedulableJobZanox: CategoriesSchedulableJob
    {
        public CategoriesSchedulableJobZanox(ICategoryRepository repositoryFromPartner, ICategoryRepository cuponicoRepository) : base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}
