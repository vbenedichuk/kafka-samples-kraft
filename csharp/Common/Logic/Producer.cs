using Common.Abstratctions;
using Common.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Common.Logic
{
    internal class Producer : IProducer
    {
        private readonly ILogger<Producer> _logger;
        private readonly IProducer<Null, string> _producer;
        private bool isDisposed;
        public Producer(ILogger<Producer> logger, IOptions<KafkaConectionSettings> settings) 
        { 
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = settings.Value.BootStrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendAsync(string topic, string message)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            } 
            catch(ProduceException<Null,string> ex)
            {
                _logger.LogError(ex, "Unable to send message. Mesage: {message}.", message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                _producer?.Dispose();
            }
            isDisposed = true;
        }
    }
}
