using Cuponico.Ingestor.Host.Domain.Tickets;

namespace Cuponico.Ingestor.Host.Domain.Jobs
{
    public sealed class CouponsSchedulableJobLomadee: CouponsSchedulableJob
    {
        public CouponsSchedulableJobLomadee(ICouponRepository repositoryFromPartner, ICouponRepository cuponicoRepository): base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}