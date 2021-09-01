using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriber
{
    class SubscriberService : IHostedService, IDisposable
    {
        private readonly IOptions<RedisOption> _options;
        private readonly ILogger<SubscriberService> _logger;

        public SubscriberService(IOptions<RedisOption> options, ILogger<SubscriberService> logger)
        {
            _options = options;
            _logger = logger;
        }
        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            using (var redisConsumer = new RedisClient(_options.Value.Host, _options.Value.Port))
            using (var subscription = redisConsumer.CreateSubscription())
            {
                subscription.OnSubscribe = channel =>
                {
                    _logger.LogInformation("Subscribed to '{0}'", channel);
                };
                subscription.OnUnSubscribe = channel =>
                {
                    _logger.LogInformation("UnSubscribed from '{0}'", channel);
                };
                subscription.OnMessage = (channel, msg) =>
                {
                    _logger.Log(LogLevel.Information, $"Got message: {msg}");
                    if (cancellationToken.IsCancellationRequested)
                    {
                        subscription.UnSubscribeFromAllChannels();
                    }
                };

                subscription.SubscribeToChannels(_options.Value?.InputQueueName); //blocking
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }

}
