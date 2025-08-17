using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<ConsumerService>();

await builder.Build().RunAsync();

public class ConsumerService : BackgroundService
{
    private readonly IConnection _connection;
    public ConsumerService(IConnection connection) => _connection = connection;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare("myqueue", durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var msg = JsonSerializer.Deserialize<JsonElement>(body);
            Console.WriteLine($"[ModuleB] Received: {msg.GetProperty("Text").GetString()} at {msg.GetProperty("Timestamp")}");
        };

        channel.BasicConsume(queue: "myqueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
