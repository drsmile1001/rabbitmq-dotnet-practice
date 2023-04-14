using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQPractice;

public class Receiver : BackgroundService
{
    private readonly string _host;
    private readonly string _virtualHost;
    private readonly string _userName;
    private readonly string _password;

    public Receiver(IConfiguration configuration)
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
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "hello2", durable: true, exclusive: false, autoDelete: false, arguments: null);


        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
        };
        channel.BasicConsume(queue: "hello2",
                            autoAck: true,
                            consumer: consumer);
        await Task.Delay(-1, stoppingToken);
    }
}
