using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQPractice;

var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                //  services.AddHostedService<Sender>();
                services.AddHostedService<Receiver>();
            });
var app = builder.Build();
await app.RunAsync();