using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    internal class PublishService : IHostedService, IDisposable
    {
        private readonly IOptions<RedisOption> _options;
        private readonly ILogger<PublishService> _logger;

        public PublishService(IOptions<RedisOption> options, ILogger<PublishService> logger)
        {
            _options = options;
            _logger = logger;
        }

        public void Dispose()
        {
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            using (var redisPublisher = new RedisClient(_options.Value.Host, _options.Value.Port))
            {

                var c = 1;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = $"Hi, I'm message nr {c.ToString()}";
                    redisPublisher.PublishMessage(_options.Value?.InputQueueName, message);
                    _logger.LogInformation($"Publishing message nr {c} to message queue...");
                    await Task.Delay(10000);
                    c++;
                }

            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }


}
