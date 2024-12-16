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
    }
}
