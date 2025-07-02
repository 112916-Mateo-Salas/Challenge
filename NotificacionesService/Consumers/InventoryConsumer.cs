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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            Console.WriteLine($"Ingreso a InventoryConsumer de mi NotificacionesService ");

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("inventory_exchange", ExchangeType.Direct,durable:true);

            string[] routingKeys = { "product.created", "product.updated", "product.deleted" };

            foreach (var key in routingKeys)
            {
                await channel.QueueDeclareAsync(queue: key, durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(queue: key, exchange: "inventory_exchange", routingKey: key);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    Console.WriteLine($"[✔] Recibido evento '{eventArgs.RoutingKey}': {json}");

                    using var scope = _scopeFactory.CreateScope();
                    var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();

                    var log = new InventoryLog
                    {
                        EventType = eventArgs.RoutingKey,
                        ProductJson = json
                    };

                    await logRepository.AddLog(log);
                    await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                };

                await channel.BasicConsumeAsync(queue: key, autoAck: false, consumer: consumer);

                
            }

            var completion = new TaskCompletionSource();
            stoppingToken.Register(() => completion.SetResult());
            await completion.Task;


        }
    }
}
