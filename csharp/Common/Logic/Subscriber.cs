using Common.Abstratctions;
using Common.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Logic
{
    internal class Subscriber : ISubscriber
    {
        private readonly KafkaConectionSettings _settings;
        public Subscriber(IOptions<KafkaConectionSettings> settings) 
        { 
            _settings = settings.Value;
        }

        /// <summary>
        /// Start receiving messages from Kafka and 
        /// </summary>
        /// <param name="topics">List of topics to subscribe</param>
        /// <param name="processFunction">Function to process received message</param>
        /// <param name="cancellationToken">Cancelation token</param>
        /// <returns>Message processing task</returns>
        public Task SubscribeAsync(string[] topics, Func<string?, Task> processFunction, CancellationToken cancellationToken)
        {
            return Task.Run(async () => {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _settings.BootStrapServers,
                    GroupId = _settings.ConsumptionGroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    // EnableAutoCommit = true,
                    // AutoCommitIntervalMs = 5000
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {

                    consumer.Subscribe(topics);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        // handle consumed message.
                        if (processFunction != null)
                        {
                            await processFunction(consumeResult.Message?.Value);
                        }
                        // consumer.Commit(); 
                    }
                    consumer.Close();
                }
            }, cancellationToken);
        }
    }
}
