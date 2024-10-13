
using ApiService.Abstractions;
using Common.Abstratctions;
using Common.Data;
using System.Text.Json;
using System.Threading;

namespace ApiService.Logic
{
    public class ResponseProcessor : IHostedService
    {
        private readonly ILogger<ResponseProcessor> _logger;
        private readonly ITopicsManager _topicsManager;
        private readonly ISubscriber _subscriber;
        private readonly IResponseCache _responseCache;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? _subscriberTask;

        public ResponseProcessor(ILogger<ResponseProcessor> logger, ITopicsManager topicsManager, ISubscriber subscriber,
            IResponseCache responseCache)
        {
            _logger = logger;
            _topicsManager = topicsManager;
            _subscriber = subscriber;
            _responseCache = responseCache;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //We should make sure that topic exists before using it.
            var requestTopicCreation = _topicsManager.CreateIfNotExistsAsync(CommonConstants.REQUEST_TOPIC_NAME);
            var responseTopicCreation = _topicsManager.CreateIfNotExistsAsync(CommonConstants.RESPONSE_TOPIC_NAME);
            await Task.WhenAll(requestTopicCreation, responseTopicCreation);

            _subscriberTask = _subscriber.SubscribeAsync([CommonConstants.RESPONSE_TOPIC_NAME], Task (value) =>
            {
                //Do meaningful work here.
                if (value != null)
                {
                    try
                    {
                        var messageObject = JsonSerializer.Deserialize<WeatherResponseMessage>(value);
                        if (messageObject != null)
                        {
                            _responseCache.SaveResponse(messageObject);
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Unable to deserialize message. Message text: `{message}`", value);
                    }
                }
                return Task.CompletedTask;
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
