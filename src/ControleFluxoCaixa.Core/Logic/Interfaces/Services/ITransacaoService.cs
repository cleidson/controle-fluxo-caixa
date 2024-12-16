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
        Task<IEnumerable<Transacao>> GetTransacoesAsync();
        Task<Transacao?> GetTransacaoByIdAsync(int id);
        Task AddTransacaoAsync(Transacao transacao);
        Task UpdateTransacaoAsync(Transacao transacao);
        Task DeleteTransacaoAsync(int id);
    }
}
