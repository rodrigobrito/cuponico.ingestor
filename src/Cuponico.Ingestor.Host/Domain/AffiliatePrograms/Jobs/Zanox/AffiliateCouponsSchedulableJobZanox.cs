using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Tickets;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Zanox
{
    public sealed class AffiliateCouponsSchedulableJobZanox : AffiliateCouponsSchedulableJob
    {
        public AffiliateCouponsSchedulableJobZanox(IAffiliateCouponRepository repositoryFromPartner, IAffiliateCouponRepository cuponicoRepository): base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}