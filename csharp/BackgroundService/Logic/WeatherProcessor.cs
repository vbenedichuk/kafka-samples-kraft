using BackgroundService.Abstractions;
using Common.Abstratctions;
using Common.Data;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BackgroundService.Logic
{
    internal class WeatherProcessor : IWeatherProcessor
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherProcessor> _logger;
        private readonly ITopicsManager _topicsManager;
        private readonly ISubscriber _subscriber;
        private readonly IProducer _producer;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? _subscriberTask;
        public WeatherProcessor(ILogger<WeatherProcessor> logger, ITopicsManager topicsManager, ISubscriber subscriber, IProducer producer) 
        { 
            _logger = logger;
            _topicsManager = topicsManager;
            _subscriber = subscriber;
            _producer = producer;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //We should make sure that topic exists before using it.
            var requestTopicCreation = _topicsManager.CreateIfNotExistsAsync(CommonConstants.REQUEST_TOPIC_NAME);
            var responseTopicCreation = _topicsManager.CreateIfNotExistsAsync(CommonConstants.RESPONSE_TOPIC_NAME);
            await Task.WhenAll(requestTopicCreation, responseTopicCreation);
            _subscriberTask = _subscriber.SubscribeAsync([CommonConstants.REQUEST_TOPIC_NAME], async Task (value) =>
                {
                    //Do meaningful work here.
                    if (value != null)
                    {
                        try
                        {
                            var request = JsonSerializer.Deserialize<WeatherRequestMessage>(value);
                            if (request != null)
                            {
                                // Simulating long running operation.
                                await Task.Delay(Random.Shared.Next(10000));
                                var response = new WeatherResponseMessage
                                {
                                    RequestId = request.RequestId,
                                    WeatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                                    {
                                        Date = DateTime.Now.Date.AddDays(index),
                                        TemperatureC = Random.Shared.Next(-20, 55),
                                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                                    })
                                    .ToArray()                                    
                                };
                                await _producer.SendAsync(CommonConstants.RESPONSE_TOPIC_NAME, JsonSerializer.Serialize(response));
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Unable to deserialize message. Message text: `{message}`", value);
                        }
                    }
                }, _cancellationTokenSource.Token);

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            if (_subscriberTask != null)
            {
                await _subscriberTask.WaitAsync(cancellationToken);
            }
        }
    }
}
