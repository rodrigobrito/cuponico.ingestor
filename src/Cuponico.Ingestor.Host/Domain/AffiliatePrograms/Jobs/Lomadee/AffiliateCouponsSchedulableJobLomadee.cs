using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Lomadee
{
    public sealed class AffiliateCouponsSchedulableJobLomadee: AffiliateCouponsSchedulableJob
    {
        public AffiliateCouponsSchedulableJobLomadee(IAffiliateCouponRepository repositoryFromPartner, IAffiliateCouponRepository cuponicoRepository, IPublisher publisher): base(repositoryFromPartner, cuponicoRepository, publisher)
        {
        }
    }
}