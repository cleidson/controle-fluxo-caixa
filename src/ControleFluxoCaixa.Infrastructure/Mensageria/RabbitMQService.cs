using ControleFluxoCaixa.Core.Logic.Enums;
using ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.Mensageria
{
    public class RabbitMQService : IMessageQueueService
    {
        private readonly ConnectionFactory _factory;
        public RabbitMQService(string hostname, string username, string password)
        {
            _factory = new ConnectionFactory
            {
                HostName = hostname,
                UserName = username,
                Password = password
            };
        }

        public async Task PublishAsync<T>(QueueName queueName, T message) where T : class
        {
            string queue = queueName.ToString();

            // Conexão e canal
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // Declaração da fila
            await channel.QueueDeclareAsync(queue, true, false, false, null);

            // Serializa e publica a mensagem
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"Mensagem publicada na fila '{queue}': {JsonSerializer.Serialize(message)}");
        }

        public async Task ConsumeAsync<T>(QueueName queueName, Func<T, Task> onMessageReceived) where T : class
        {
            var queue = queueName.ToString();
            int retryCount = 0;

            while (true)
            {
                try
                {
                    // Tentar conexão
                    var connection = await _factory.CreateConnectionAsync();
                    var channel = await connection.CreateChannelAsync();

                    await channel.QueueDeclareAsync(queue, true, false, false, null);

                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));

                            if (message != null)
                            {
                                await onMessageReceived(message);
                                await channel.BasicAckAsync(ea.DeliveryTag, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                            await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                        }
                    };

                    await channel.BasicConsumeAsync(queue, false, $"{queue}-consumer-{Guid.NewGuid()}", consumer);

                    Console.WriteLine($"Consumidor conectado para a fila '{queue}'.");
                    break;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                {
                    retryCount++;
                    Console.WriteLine($"Tentativa {retryCount}: Conexão com RabbitMQ falhou. Tentando novamente em 5 segundos...");
                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    throw;
                }
            }
        }

    }
}
