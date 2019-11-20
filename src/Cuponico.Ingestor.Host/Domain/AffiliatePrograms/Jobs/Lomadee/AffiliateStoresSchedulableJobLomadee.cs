using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Lomadee
{
    public class AffiliateStoresSchedulableJobLomadee : AffiliateStoresSchedulableJob
    {
        public AffiliateStoresSchedulableJobLomadee(IAffiliateStoreRepository repositoryFromPartner, IAffiliateStoreRepository cuponicoRepository, IPublisher publisher) : base(repositoryFromPartner, cuponicoRepository, publisher)
        {
        }
    }
}