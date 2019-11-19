using Cuponico.Ingestor.Host.Domain.Stores;

namespace Cuponico.Ingestor.Host.Domain.Jobs.Zanox
{
    public class StoresSchedulableJobZanox : StoresSchedulableJob
    {
        public StoresSchedulableJobZanox(IStoreRepository repositoryFromPartner, IStoreRepository cuponicoRepository) : base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}