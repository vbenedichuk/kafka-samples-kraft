using System;
using System.Collections.Generic;

namespace Common.Data
{
    public class WeatherResponseMessage
    {
        public Guid RequestId { get; set; }
        public IEnumerable<WeatherForecast> WeatherForecasts { get; set; }
    }
}
