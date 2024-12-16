using ControleFluxoCaixa.Core.Logic.Exceptions;
using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Core.Logic.Interfaces.Services;
using ControleFluxoCaixa.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransactionalRepository<Transacao> _repository;
        private readonly ITransactionalRepository<SaldoDiario> _saldoRepository;

        public TransacaoService(ITransactionalRepository<Transacao> repository, ITransactionalRepository<SaldoDiario> saldoRepository)
        {
            _repository = repository;
            _saldoRepository = saldoRepository;
        }

        public async Task AddTransacaoAsync(Transacao transacao)
        {
            // Validação da transação
            if (transacao.Valor <= 0)
                throw new TransacaoInvalidaException("O valor da transação deve ser maior que zero.");

            // Processar saldo, se necessário
            if (transacao.Tipo == "Debito")
                await ValidarEAtualizarSaldo(transacao.UsuarioId, -transacao.Valor);

            if (transacao.Tipo == "Credito")
                await ValidarEAtualizarSaldo(transacao.UsuarioId, transacao.Valor);

            // Adicionar transação
            await _repository.AddAsync(transacao);
        }

        private async Task ValidarEAtualizarSaldo(int usuarioId, decimal ajuste)
        {
            // Obter saldo do usuário
            var saldoDiario = (await _saldoRepository.FindAsync(s => s.UsuarioId == usuarioId)).FirstOrDefault();

            if (ajuste < 0) // Débito
            {
                if (saldoDiario == null || saldoDiario.SaldoAtual + ajuste < 0)
                    throw new SaldoInsuficienteException("Saldo insuficiente para realizar a transação.");
            }

            if (saldoDiario != null)
            {
                saldoDiario.SaldoAtual += ajuste;
                await _saldoRepository.UpdateAsync(saldoDiario);
            }
            else
            {
                // Criar novo registro de saldo, caso não exista
                await _saldoRepository.AddAsync(new SaldoDiario
                {
                    UsuarioId = usuarioId,
                    SaldoAtual = ajuste,
                    DataHora = DateTime.UtcNow
                });
            }
        }

        public async Task AtualizarSaldoDiarioAsync(int usuarioId, decimal valor)
        {
            await ValidarEAtualizarSaldo(usuarioId, valor);
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
