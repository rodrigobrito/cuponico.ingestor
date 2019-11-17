using Cuponico.Ingestor.Host.Domain.Categories;
using Cuponico.Ingestor.Host.Jobs;

namespace Cuponico.Ingestor.Host.Domain.Jobs
{
    public class CategoriesSchedulableJobLomadee : CategoriesSchedulableJob
    {
        public CategoriesSchedulableJobLomadee(ICategoryRepository repositoryFromPartner, ICategoryRepository cuponicoRepository) : base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}
