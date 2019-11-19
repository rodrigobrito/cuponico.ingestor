using Cuponico.Ingestor.Host.Domain.Tickets;

namespace Cuponico.Ingestor.Host.Domain.Jobs.Zanox
{
    public sealed class CouponsSchedulableJobZanox : CouponsSchedulableJob
    {
        public CouponsSchedulableJobZanox(ICouponRepository repositoryFromPartner, ICouponRepository cuponicoRepository): base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}