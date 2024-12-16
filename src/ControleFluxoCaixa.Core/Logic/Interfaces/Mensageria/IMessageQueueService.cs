using ControleFluxoCaixa.Core.Logic.Enums;

namespace ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria
{
    public interface IMessageQueueService
    {
        /// <summary>
        /// Publica uma mensagem na fila especificada.
        /// </summary>
        /// <typeparam name="T">O tipo de mensagem a ser publicada.</typeparam>
        /// <param name="queueName">Nome da fila, usando o Enum QueueName.</param>
        /// <param name="message">Mensagem a ser publicada.</param>
        Task PublishAsync<T>(QueueName queueName, T message) where T : class;

        /// <summary>
        /// Consome mensagens da fila especificada.
        /// </summary>
        /// <typeparam name="T">O tipo da mensagem a ser consumida.</typeparam>
        /// <param name="queueName">Nome da fila, usando o Enum QueueName.</param>
        /// <param name="onMessageReceived">
        /// Callback assíncrono que processará a mensagem recebida.
        /// Recebe o objeto deserializado como parâmetro.
        /// </param>
        Task ConsumeAsync<T>(QueueName queueName, Func<T, Task> onMessageReceived) where T : class;
    }
}
