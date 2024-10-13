using System.Threading.Tasks;
using Common.Abstratctions;
using Common.Config;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Logic
{
    internal class TopicsManager : ITopicsManager
    {
        private readonly KafkaConectionSettings _settings;
        private readonly ILogger<TopicsManager> _logger;
        public TopicsManager(ILogger<TopicsManager> logger, IOptions<KafkaConectionSettings> settings) 
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task CreateIfNotExistsAsync(string topicName, int partitionsCount)
        {
            var clientConfig = new AdminClientConfig{
                BootstrapServers = _settings.BootStrapServers                
            };

            var adminBuilder = new AdminClientBuilder(clientConfig);
            using(var adminClient = adminBuilder.Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new[] { new TopicSpecification{
                        Name = topicName
                    }});                    
                } catch(CreateTopicsException ex)
                {
                    _logger.LogError(ex, "Unable to create topic. Topic name {name}", topicName);
                }
            }
        }

        public Task CreateIfNotExistsAsync(string topicName)
        {
            return CreateIfNotExistsAsync(topicName, _settings.DefaultPartitionsCount);
        }
    }
}
