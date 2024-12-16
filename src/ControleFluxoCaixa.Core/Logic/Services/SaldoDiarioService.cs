using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Core.Logic.Interfaces.Services;
using ControleFluxoCaixa.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Services
{
    public class SaldoDiarioService : ISaldoDiarioService
    {
        private readonly IRepository<SaldoDiario> _repository;

        public SaldoDiarioService(IRepository<SaldoDiario> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SaldoDiario>> GetConsolidadoDiarioAsync(int usuarioId, DateTime data)
        {
            return await _repository.FindAsync(s =>
                s.UsuarioId == usuarioId && s.DataHora.Date == data.Date);
        }

        public async Task AtualizarSaldoDiarioAsync(int usuarioId, decimal valor)
        {
            var saldo = await _repository.FindAsync(s => s.UsuarioId == usuarioId);
            var saldoAtual = saldo.FirstOrDefault();

            if (saldoAtual != null)
            {
                saldoAtual.SaldoAtual += valor;
                await _repository.UpdateAsync(saldoAtual);
            }
            else
            {
                await _repository.AddAsync(new SaldoDiario
                {
                    UsuarioId = usuarioId,
                    SaldoAtual = valor,
                    DataHora = DateTime.UtcNow
                });
            }
        }
    }
}
