using Common.Config;
using BackgroundService.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Common.Abstratctions;

namespace BackgroundService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {                    
                    services.AddKafka(hostContext.Configuration);
                    services.AddHostedService<WeatherProcessor>();
                });
    }
}
