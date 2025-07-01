using InventoryService.Messaging.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryService.Messaging.Impl
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly IConfiguration _config;

        public RabbitPublisher(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task Publish(object data, string routingKey)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("inventory_exchange", ExchangeType.Direct, durable: true);
            var json = JsonSerializer.Serialize(data);
            var body = JsonSerializer.SerializeToUtf8Bytes(json);
            await channel.BasicPublishAsync("inventory_exchange",routingKey, body);
        }
    }
}
