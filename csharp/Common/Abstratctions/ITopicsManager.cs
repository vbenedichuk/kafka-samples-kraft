using System.Threading.Tasks;

namespace Common.Abstratctions
{
    public interface ITopicsManager
    {
        Task CreateIfNotExistsAsync(string topicName, int partitionsCount);
        Task CreateIfNotExistsAsync(string topicName);
    }
}
