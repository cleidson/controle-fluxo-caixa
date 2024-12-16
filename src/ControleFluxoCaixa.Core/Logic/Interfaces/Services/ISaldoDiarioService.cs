using ControleFluxoCaixa.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Interfaces.Services
{
    public interface ISaldoDiarioService
    {
        Task<IEnumerable<SaldoDiario>> GetConsolidadoDiarioAsync(int usuarioId, DateTime data);
        Task AtualizarSaldoDiarioAsync(int usuarioId, decimal valor);
    }
}
