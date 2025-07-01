using NotificacionesService.Models;
using NotificacionesService.Repositories.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificacionesService.Consumers
{
    public class InventoryConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public InventoryConsumer(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _config = configuration;
            _scopeFactory = serviceScopeFactory;
        }

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
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

            await channel.ExchangeDeclareAsync("inventory_exchange", ExchangeType.Direct);

            string[] routingKeys = { "product.created", "product.updated", "product.deleted" };

            foreach (var key in routingKeys)
            {
                await channel.QueueDeclareAsync(queue: key, durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(queue: key, exchange: "inventory_exchange", routingKey: key);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ILogRepository>();

                    var log = new InventoryLog
                    {
                        EventType = ea.RoutingKey,
                        ProductJson = json
                    };

                    await repo.AddLog(log);
                };

                await channel.BasicConsumeAsync(queue: key, autoAck: true, consumer: consumer);
            }

            return Task.CompletedTask;
        }
    }
}
