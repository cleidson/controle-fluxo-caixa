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
            string queue = queueName.ToString(); // Converte o Enum para string

            // Criando conexão e canal assíncronos
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // Declarando a fila (idempotente, cria caso não exista)
            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Serializando o objeto para JSON
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            // Criando as propriedades básicas (mensagem persistente)
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };

            // Publicando a mensagem
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"Mensagem publicada com sucesso para a fila '{queue}': {JsonSerializer.Serialize(message)}");
        }

        public async Task ConsumeAsync<T>(QueueName queueName, Func<T, Task> onMessageReceived) where T : class
        {
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName.ToString(),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

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
                        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName.ToString(),
                autoAck: false, // Controle manual de confirmação
                consumer: consumer
            );

            Console.WriteLine($"Consumindo mensagens da fila '{queueName}'.");
        }
    }
}
