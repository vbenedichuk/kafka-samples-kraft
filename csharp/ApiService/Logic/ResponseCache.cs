using ApiService.Abstractions;
using Common.Data;
using System.Collections.Concurrent;

namespace ApiService.Logic
{
    public class ResponseCache : IResponseCache
    {
        private readonly ConcurrentDictionary<Guid, WeatherResponseMessage> _responses = new ConcurrentDictionary<Guid, WeatherResponseMessage>();
        public WeatherResponseMessage? GetResponse(Guid requestId)
        {
            _responses.TryGetValue(requestId, out WeatherResponseMessage? response);
            return response;
        }

        public void SaveResponse(WeatherResponseMessage response)
        {
            _responses.AddOrUpdate(response.RequestId, response, (oldValue, newValue) => newValue);
        }
    }
}
