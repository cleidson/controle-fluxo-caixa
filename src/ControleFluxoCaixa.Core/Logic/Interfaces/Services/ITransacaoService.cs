using ControleFluxoCaixa.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Interfaces.Services
{
    public interface ITransacaoService
    { 
        Task AddTransacaoAsync(Transacao transacao);
        Task UpdateTransacaoAsync(Transacao transacao);
        Task DeleteTransacaoAsync(int id);

        Task AtualizarSaldoDiarioAsync(int usuarioId, decimal valor);
    }
}
