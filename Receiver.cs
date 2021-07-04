using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RabbitMQPractice
{
    public class Receiver : BackgroundService
    {
        private readonly ILogger<Receiver> _logger;

        public Receiver(ILogger<Receiver> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Worker("A", "A1", stoppingToken);
            _ = Worker("A", "A2", stoppingToken);
            _ = Worker("B", "B1", stoppingToken);
            _ = Worker("B", "B2", stoppingToken);
            return Task.CompletedTask;
        }

        private async Task Worker(string type, string id, CancellationToken stoppingToken)
        {
            using var bus = RabbitHutch.CreateBus("host=localhost;prefetchcount=1");
            var exchange = await bus.Advanced.ExchangeDeclareAsync("event", configure: c => c.WithType(ExchangeType.Fanout));
            var queueName = $"event_{type}";
            var queue = await bus.Advanced.QueueDeclareAsync(queueName, c => c.AsDurable(true).AsExclusive(false).AsAutoDelete(false));
            bus.Advanced.Bind(exchange: exchange, queue: queue, routingKey: "");
            using var consume = bus.Advanced.Consume(queue: queue, (body, properties, info) => Task.Run(async () =>
            {
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("{id} 收到事件 {message}", id, message);
                await Task.Delay(1200);
                _logger.LogInformation("{id} 完成事件 {message}", id, message);
            }));
            await Task.Delay(-1, stoppingToken);
        }
    }
}
