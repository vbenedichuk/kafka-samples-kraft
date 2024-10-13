using Common.Data;

namespace ApiService.Abstractions
{
    public interface IResponseCache
    {
        void SaveResponse(WeatherResponseMessage response);
        WeatherResponseMessage? GetResponse(Guid requestId);
    }
}
