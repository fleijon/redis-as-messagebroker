namespace Subscriber
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Threading.Tasks;

    class Program
    {

        static async Task Main(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = configBuilder.Build();

            await new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<SubscriberService>();
                services.AddLogging();
                services.Configure<RedisOption>(config.GetSection("RedisOption"));
            })
            .ConfigureLogging((hostContext, configLogging) =>
            {
                configLogging.AddConsole();
                configLogging.AddDebug();
            })
            .RunConsoleAsync();
        }
    }
}
