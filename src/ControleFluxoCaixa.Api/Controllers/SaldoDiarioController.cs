using ControleFluxoCaixa.Core.Logic.Enums;
using ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria;
using ControleFluxoCaixa.Core.Logic.Interfaces.Services;
using ControleFluxoCaixa.Infrastructure.DTOs.Mensageria.Transacao;
using Microsoft.AspNetCore.Mvc;

namespace ControleFluxoCaixa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaldoDiarioController : ControllerBase
    {
        private readonly ISaldoDiarioService _saldoDiarioService;

        public SaldoDiarioController(ISaldoDiarioService saldoDiarioService)
        {
            _saldoDiarioService = saldoDiarioService;
        }

        [HttpGet("{usuarioId}/{data}")]
        public async Task<IActionResult> GetConsolidadoDiario(int usuarioId, DateTime data)
        {
            var consolidado = await _saldoDiarioService.GetConsolidadoDiarioAsync(usuarioId, data);
            return Ok(consolidado);
        }

        [HttpPost("atualizar")]
        public async Task<IActionResult> AtualizarSaldoDiario([FromBody] SaldoMessageDto request)
        {
            await _saldoDiarioService.AtualizarSaldoDiarioAsync(request.UsuarioId, request.SaldoAtual);
            return Ok();
        }
    }

}
