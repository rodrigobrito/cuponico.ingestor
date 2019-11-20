using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Stores;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Zanox
{
    public class AffiliateStoresSchedulableJobZanox : AffiliateStoresSchedulableJob
    {
        public AffiliateStoresSchedulableJobZanox(IAffiliateStoreRepository repositoryFromPartner, IAffiliateStoreRepository cuponicoRepository, IPublisher publisher) : base(repositoryFromPartner, cuponicoRepository, publisher)
        {
        }
    }
}