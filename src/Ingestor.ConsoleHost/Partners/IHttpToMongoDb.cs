using System.Threading.Tasks;

namespace Ingestor.ConsoleHost.Partners
{
    public interface IHttpToMongoDb
    {
        Task Import();
    }
}