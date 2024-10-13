using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Abstratctions
{
    public interface ISubscriber
    {
        Task SubscribeAsync(string[] topics, Func<string?, Task> processFunction, CancellationToken cancellationToken);
    }
}
