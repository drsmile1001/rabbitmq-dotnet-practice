using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace RabbitMQPractice
{
    public class Sender : BackgroundService
    {
        private readonly string _connectionString;

        public Sender(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RabbitMQ");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bus = RabbitHutch.CreateBus(_connectionString, register=>{
                register.EnableSystemTextJson();
            });
            while (!stoppingToken.IsCancellationRequested)
            {
                await bus.PubSub.PublishAsync($"Now: {DateTime.Now}",configure=>{
                    configure.WithTopic("my_topic");
                },stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
