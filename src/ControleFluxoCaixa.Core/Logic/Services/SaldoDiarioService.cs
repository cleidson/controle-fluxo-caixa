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
        private readonly IReadOnlyDataRepository<SaldoDiario> _repository;

        public SaldoDiarioService(IReadOnlyDataRepository<SaldoDiario> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SaldoDiario>> GetConsolidadoDiarioAsync(int usuarioId, DateTime data)
        {
            return await _repository.FindAsync(s =>
                s.UsuarioId == usuarioId && s.DataHora.Date == data.Date);
        }

    }
}
