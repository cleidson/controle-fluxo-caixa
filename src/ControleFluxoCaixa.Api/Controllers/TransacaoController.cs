using ControleFluxoCaixa.Core.Logic.Enums;
using ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria;
using ControleFluxoCaixa.Infrastructure.DTOs.Mensageria.Transacao;
using Microsoft.AspNetCore.Mvc;

namespace ControleFluxoCaixa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly IMessageQueueService _messageQueueService;

        public TransacaoController(IMessageQueueService messageQueueService)
        {
            _messageQueueService = messageQueueService;
        }

        /// <summary>
        /// Publica uma transação na fila "FilaTransacao".
        /// </summary>
        [HttpPost("transacao")]
        public async Task<IActionResult> PublicarTransacao([FromBody] TransacaoMessageDto transacaoDto)
        {
            if (transacaoDto == null)
            {
                return BadRequest("O DTO de transação não pode ser nulo.");
            }

            await _messageQueueService.PublishAsync(QueueName.FilaTransacao, transacaoDto);

            return Ok($"Transação publicada com sucesso para o usuário {transacaoDto.UsuarioId}.");
        }

        /// <summary>
        /// Publica um saldo na fila "FilaSaldos".
        /// </summary>
        [HttpPost("saldo")]
        public async Task<IActionResult> PublicarSaldo([FromBody] SaldoMessageDto saldoDto)
        {
            if (saldoDto == null)
            {
                return BadRequest("O DTO de saldo não pode ser nulo.");
            }

            await _messageQueueService.PublishAsync(QueueName.FilaSaldos, saldoDto);

            return Ok($"Saldo atualizado publicado com sucesso para o usuário {saldoDto.UsuarioId}.");
        }
    }
}
