using Common.Abstratctions;
using Common.Logic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Config
{
    public static class KafkaConfigExtension
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaConectionSettings>(configuration.GetSection("KafkaConnection"));
            services.AddSingleton<ITopicsManager, TopicsManager>();
            services.AddSingleton<IProducer, Producer>();
            services.AddSingleton<ISubscriber, Subscriber>();
            return services;
        }
    }
}
