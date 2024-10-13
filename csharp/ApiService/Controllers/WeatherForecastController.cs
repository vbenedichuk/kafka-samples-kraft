using ApiService.Abstractions;
using ApiService.Data;
using Common.Abstratctions;
using Common.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IProducer _producer;
        private readonly IResponseCache _responseCache;

        public WeatherForecastController(IProducer producer, IResponseCache responseCache)
        {
            _producer = producer;
            _responseCache = responseCache;
        }

        /// <summary>
        /// Send request to the background service.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(RequestIdResponse), StatusCodes.Status200OK)]
        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            var request = new WeatherRequestMessage
            {
                RequestId = Guid.NewGuid()
            };
            _producer.SendAsync(CommonConstants.REQUEST_TOPIC_NAME, JsonSerializer.Serialize(request));
            return Ok(new RequestIdResponse
            {
                Id = request.RequestId
            });
        }

        /// <summary>
        /// Returns response if available
        /// </summary>
        /// <param name="id">Request id</param>
        /// <returns>Weather forecast or 404 if request is not processed yet.</returns>
        [HttpGet("/{id:guid}")]
        [ProducesResponseType(typeof(IList<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetForecast(Guid id)
        {
            var response = _responseCache.GetResponse(id);
            if(response == null)
            {
                return NotFound();
            }
            return Ok(response.WeatherForecasts);
        }
    }
}

