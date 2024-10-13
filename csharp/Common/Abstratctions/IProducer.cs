using System;
using System.Threading.Tasks;

namespace Common.Abstratctions
{
    public interface IProducer : IDisposable
    {
        Task SendAsync(string topic, string message);
    }
}
