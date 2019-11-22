using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Lomadee
{
    public class AffiliateCategoriesSchedulableJobLomadee : AffiliateCategoriesSchedulableJob
    {
        public AffiliateCategoriesSchedulableJobLomadee(IAffiliateCategoryRepository repositoryFromPartner, IAffiliateCategoryRepository cuponicoRepository, IPublisher publisher) : base(repositoryFromPartner, cuponicoRepository, publisher)
        {
        }
    }
}
