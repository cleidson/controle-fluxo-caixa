using ControleFluxoCaixa.Core.Logic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
