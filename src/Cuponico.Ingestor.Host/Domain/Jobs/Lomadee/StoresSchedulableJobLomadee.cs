using Cuponico.Ingestor.Host.Domain.Stores;

namespace Cuponico.Ingestor.Host.Domain.Jobs.Lomadee
{
    public class StoresSchedulableJobLomadee : StoresSchedulableJob
    {
        public StoresSchedulableJobLomadee(IStoreRepository repositoryFromPartner, IStoreRepository cuponicoRepository) : base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}
