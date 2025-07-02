using InventoryService.Messaging.Interfaces;
using Polly;
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
            //IMPLEMENTACION CIRCUIT BRAKER
            //Implementación de POLLY, se abre el circuito despues de 2 fallos consecutivos
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(15),
                onBreak: (ex, breakDelay) =>
                {
                    Console.WriteLine($"Circuit Breaker ON. Circuito abierto por {breakDelay.TotalSeconds}s... por error: {ex.Message}");
                },
                onReset: () => Console.WriteLine("Circuit Breaker OFF. Circuito cerrado, se reanuda publicación."),
                onHalfOpen: () => Console.WriteLine("Circuit breaker Half-OPEN. Circuito semi-abierto estado de prueba.")
                );
            

            await circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();
                await channel.ExchangeDeclareAsync("inventory_exchange", ExchangeType.Direct, durable: true);
                var json = JsonSerializer.Serialize(data);
                var body = JsonSerializer.SerializeToUtf8Bytes(data);
                await channel.BasicPublishAsync("inventory_exchange", routingKey, body);
                Console.WriteLine($"[✔] Evento publicado en '{routingKey}': {json}");
            });
            

        }
    }
}
