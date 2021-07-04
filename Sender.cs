using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace RabbitMQPractice
{
    public class Sender : BackgroundService
    {
        private readonly ILogger<Sender> _logger;

        public Sender(ILogger<Sender> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=true");
            var exchange = await bus.Advanced.ExchangeDeclareAsync("event", configure: c => c.WithType(ExchangeType.Fanout));

            for (int i = 1; !stoppingToken.IsCancellationRequested; i++)
            {
                var payload = $"{i}";
                var message = new Message<string>(payload, new MessageProperties());
                await bus.Advanced.PublishAsync(exchange: exchange, routingKey: "", mandatory: true, message);
                _logger.LogInformation("發佈事件 {payload}", payload);
                await Task.Delay(1000);
            }
        }
    }
}
