using Cuponico.Ingestor.Host.Domain.Tickets;

namespace Cuponico.Ingestor.Host.Domain.Jobs.Lomadee
{
    public sealed class CouponsSchedulableJobLomadee: CouponsSchedulableJob
    {
        public CouponsSchedulableJobLomadee(ICouponRepository repositoryFromPartner, ICouponRepository cuponicoRepository): base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}