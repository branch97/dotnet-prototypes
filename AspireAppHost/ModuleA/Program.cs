using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

// Register RabbitMQ client using Aspire injection
builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<PublisherService>();

await builder.Build().RunAsync();

public class PublisherService : BackgroundService
{
    private readonly IConnection _connection;
    public PublisherService(IConnection connection) => _connection = connection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare("myqueue", durable: true, exclusive: false, autoDelete: false);

        int count = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var payload = new { Text = $"Hello from ModuleA #{++count}", Timestamp = DateTime.UtcNow };
            var bytes = JsonSerializer.SerializeToUtf8Bytes(payload);

            channel.BasicPublish(exchange: "", routingKey: "myqueue", body: bytes);
            Console.WriteLine($"[ModuleA] Sent message: {payload.Text}");
            await Task.Delay(2000, stoppingToken);
        }
    }
}
