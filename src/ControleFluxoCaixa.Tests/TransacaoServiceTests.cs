using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Core.Logic.Services;
using ControleFluxoCaixa.Core.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace ControleFluxoCaixa.Tests
{
    public class TransacaoServiceTests
    {
        private readonly Mock<ITransactionalRepository<Transacao>> _mockTransacaoRepository;
        private readonly Mock<ITransactionalRepository<SaldoDiario>> _mockSaldoRepository;
        private readonly TransacaoService _service;

        public TransacaoServiceTests()
        {
            _mockTransacaoRepository = new Mock<ITransactionalRepository<Transacao>>();
            _mockSaldoRepository = new Mock<ITransactionalRepository<SaldoDiario>>();
            _service = new TransacaoService(_mockTransacaoRepository.Object, _mockSaldoRepository.Object);
        }

        [Fact]
        public async Task AddTransacaoAsync_Should_Add_Credit_Transaction()
        {
            // Arrange
            var transacao = new Transacao { UsuarioId = 1, Valor = 100, Tipo = "Credito" };

            _mockSaldoRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<SaldoDiario, bool>>>()))
                                .ReturnsAsync(new List<SaldoDiario>
                                {
                                new SaldoDiario { UsuarioId = 1, SaldoAtual = 0 }
                                });

            // Act
            await _service.AddTransacaoAsync(transacao);

            // Assert
            _mockSaldoRepository.Verify(r => r.UpdateAsync(It.Is<SaldoDiario>(s => s.SaldoAtual == 100)), Times.Once);
            _mockTransacaoRepository.Verify(r => r.AddAsync(transacao), Times.Once);
        }

        [Fact]
        public async Task AddTransacaoAsync_Should_Add_Debit_Transaction_With_Negative_Balance()
        {
            // Arrange
            var transacao = new Transacao { UsuarioId = 1, Valor = 50, Tipo = "Debito" };

            _mockSaldoRepository
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<SaldoDiario, bool>>>()))
                .ReturnsAsync(new List<SaldoDiario> { new SaldoDiario { UsuarioId = 1, SaldoAtual = 30 } });

            // Act
            await _service.AddTransacaoAsync(transacao);

            // Assert
            _mockSaldoRepository.Verify(r => r.UpdateAsync(It.Is<SaldoDiario>(s => s.SaldoAtual == -20)), Times.Once);
            _mockTransacaoRepository.Verify(r => r.AddAsync(transacao), Times.Once);
        }

        [Fact]
        public async Task AddTransacaoAsync_Should_Create_New_SaldoDiario_If_Not_Exists()
        {
            // Arrange
            var transacao = new Transacao { UsuarioId = 1, Valor = 50, Tipo = "Credito" };

            _mockSaldoRepository
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<SaldoDiario, bool>>>()))
                .ReturnsAsync(new List<SaldoDiario>());

            // Act
            await _service.AddTransacaoAsync(transacao);

            // Assert
            _mockSaldoRepository.Verify(r => r.AddAsync(It.Is<SaldoDiario>(s => s.UsuarioId == 1 && s.SaldoAtual == 50)), Times.Once);
            _mockTransacaoRepository.Verify(r => r.AddAsync(transacao), Times.Once);
        }

        [Fact]
        public async Task AtualizarSaldoDiarioAsync_Should_Update_SaldoDiario()
        {
            // Arrange
            var usuarioId = 1;
            var valor = 50;

            _mockSaldoRepository
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<SaldoDiario, bool>>>()))
                .ReturnsAsync(new List<SaldoDiario> { new SaldoDiario { UsuarioId = 1, SaldoAtual = 20 } });

            // Act
            await _service.AtualizarSaldoDiarioAsync(usuarioId, valor);

            // Assert
            _mockSaldoRepository.Verify(r => r.UpdateAsync(It.Is<SaldoDiario>(s => s.SaldoAtual == 70)), Times.Once);
        }

        [Fact]
        public async Task UpdateTransacaoAsync_Should_Update_Transacao()
        {
            // Arrange
            var transacao = new Transacao { Id = 1, UsuarioId = 1, Valor = 100, Tipo = "Credito" };

            // Act
            await _service.UpdateTransacaoAsync(transacao);

            // Assert
            _mockTransacaoRepository.Verify(r => r.UpdateAsync(transacao), Times.Once);
        }

        [Fact]
        public async Task DeleteTransacaoAsync_Should_Delete_Transacao()
        {
            // Arrange
            var transacaoId = 1;

            // Act
            await _service.DeleteTransacaoAsync(transacaoId);

            // Assert
            _mockTransacaoRepository.Verify(r => r.DeleteAsync(transacaoId), Times.Once);
        }
    }
}
