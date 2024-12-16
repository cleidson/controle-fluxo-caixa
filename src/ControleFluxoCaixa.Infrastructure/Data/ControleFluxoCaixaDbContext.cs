using ControleFluxoCaixa.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.Data
{
    public class ControleFluxoCaixaDbContext : DbContext
    {
        public ControleFluxoCaixaDbContext(DbContextOptions<ControleFluxoCaixaDbContext> options)
            : base(options) { }

        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<SaldoDiario> SaldosDiarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automaticamente todas as configurações do Fluent API
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControleFluxoCaixaDbContext).Assembly);
        }
    }
}
