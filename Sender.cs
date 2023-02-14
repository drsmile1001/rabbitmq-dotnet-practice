using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace RabbitMQPractice;

public class Sender : BackgroundService
{
    private readonly string _host;
    private readonly string _virtualHost;
    private readonly string _userName;
    private readonly string _password;

    public Sender(IConfiguration configuration)
    {
        _host = configuration.GetValue<string>("RabbitMQ:Host");
        _virtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost");
        _userName = configuration.GetValue<string>("RabbitMQ:UserName");
        _password = configuration.GetValue<string>("RabbitMQ:Password");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _host,
            VirtualHost = _virtualHost,
            UserName = _userName,
            Password = _password
        };
        using var connection = factory.CreateConnection();
        using var channel  = connection.CreateModel();
        channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
        while (!stoppingToken.IsCancellationRequested)
        {
            string message = $"Now: {DateTime.Now}";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: string.Empty, routingKey: "hello", basicProperties: null, body: body);
            await Task.Delay(1000,stoppingToken);
        }
    }
}
