using ControleFluxoCaixa.Core.Logic.Exceptions;
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
    public class TransacaoService : ITransacaoService
    {
        private readonly IRepository<Transacao> _repository;
        private readonly IRepository<SaldoDiario> _saldoRepository;

        public TransacaoService(IRepository<Transacao> repository, IRepository<SaldoDiario> saldoRepository)
        {
            _repository = repository;
            _saldoRepository = saldoRepository;
        }

        public async Task<IEnumerable<Transacao>> GetTransacoesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Transacao?> GetTransacaoByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddTransacaoAsync(Transacao transacao)
        {
            // Validação da transação
            if (transacao.Valor <= 0)
            {
                throw new TransacaoInvalidaException("O valor da transação deve ser maior que zero.");
            }

            // Regra de negócio: verificar saldo antes de débito
            if (transacao.Tipo == "Debito")
            {
                var saldo = await _saldoRepository.FindAsync(s => s.UsuarioId == transacao.UsuarioId);
                var saldoAtual = saldo.FirstOrDefault()?.SaldoAtual ?? 0;

                if (saldoAtual < transacao.Valor)
                {
                    throw new SaldoInsuficienteException("Saldo insuficiente para realizar a transação.");
                }

                // Atualizar o saldo
                var saldoDiario = saldo.FirstOrDefault();
                if (saldoDiario != null)
                {
                    saldoDiario.SaldoAtual -= transacao.Valor;
                    await _saldoRepository.UpdateAsync(saldoDiario);
                }
            }

            // Adicionar transação
            await _repository.AddAsync(transacao);
        }

        public async Task UpdateTransacaoAsync(Transacao transacao)
        {
            await _repository.UpdateAsync(transacao);
        }

        public async Task DeleteTransacaoAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
