using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RabbitMQPractice
{
    public class Receiver : BackgroundService
    {
        private readonly ILogger<Receiver> _logger;
        private readonly string _connectionString;
        private SubscriptionResult _subscription;

        public Receiver(ILogger<Receiver> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("RabbitMQ");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bus = RabbitHutch.CreateBus(_connectionString, register=>{
                register.EnableSystemTextJson();
            });
            _subscription = await bus.PubSub.SubscribeAsync<string>("my_subscription_id", msg =>
            {
                _logger.LogInformation("Received: {msg}", msg);
            }, configure=>{
                configure.WithQueueName("my_queue_name");
            }, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _subscription.Dispose();
            await base.StopAsync(stoppingToken);
        }
    }
}
