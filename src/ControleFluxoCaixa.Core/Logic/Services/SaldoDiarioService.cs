using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Core.Logic.Interfaces.Services;
using ControleFluxoCaixa.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Services
{
    /// <summary>
    /// Classe responsável por fornecer operações relacionadas ao saldo diário consolidado.
    /// 
    /// A classe segue os princípios SOLID:
    /// - SRP (Single Responsibility Principle): A responsabilidade única desta classe é buscar dados consolidados do saldo diário.
    /// - OCP (Open/Closed Principle): A classe depende de uma abstração (IReadOnlyDataRepository), permitindo a extensão sem modificação.
    /// - LSP (Liskov Substitution Principle): Pode aceitar qualquer implementação de IReadOnlyDataRepository sem impactar seu comportamento.
    /// - ISP (Interface Segregation Principle): Depende de uma interface específica e segregada, focada apenas em operações de leitura.
    /// - DIP (Dependency Inversion Principle): Depende da abstração IReadOnlyDataRepository, mantendo o código desacoplado e testável.
    /// </summary>
    public class SaldoDiarioService : ISaldoDiarioService
    {
        // Repositório de dados somente leitura injetado, aplicando DIP (Dependency Inversion Principle)
        private readonly IReadOnlyDataRepository<SaldoDiario> _repository;

        /// <summary>
        /// Construtor da classe SaldoDiarioService.
        /// 
        /// Injeta uma implementação de IReadOnlyDataRepository, mantendo o código desacoplado.
        /// </summary>
        /// <param name="repository">Repositório de dados somente leitura para SaldoDiario.</param>
        public SaldoDiarioService(IReadOnlyDataRepository<SaldoDiario> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Obtém o saldo diário consolidado para um usuário em uma data específica.
        /// 
        /// Método respeita SRP ao focar apenas nesta operação específica.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <param name="data">Data para a qual o saldo consolidado será buscado.</param>
        /// <returns>Uma lista de registros de SaldoDiario.</returns>
        /// <exception cref="ArgumentException">Lançado se o ID do usuário for inválido.</exception>
        public async Task<IEnumerable<SaldoDiario>> GetConsolidadoDiarioAsync(int usuarioId, DateTime data)
        {
            // Validação básica de parâmetro, reforçando a qualidade do código.
            if (usuarioId <= 0)
                throw new ArgumentException("O ID do usuário deve ser maior que zero.", nameof(usuarioId));

            // Busca os dados no repositório, aplicando a responsabilidade única desta classe.
            return await _repository.FindAsync(s =>
                s.UsuarioId == usuarioId && s.DataHora.Date == data.Date);
        }
    }
}
