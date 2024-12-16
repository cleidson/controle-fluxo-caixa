using ControleFluxoCaixa.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.Data
{
    public class ControleFluxoCaixaReadOnlyDbContext : DbContext
    {
        public ControleFluxoCaixaReadOnlyDbContext(DbContextOptions<ControleFluxoCaixaReadOnlyDbContext> options)
            : base(options) { }

        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<SaldoDiario> SaldosDiarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControleFluxoCaixaReadOnlyDbContext).Assembly);
        }
    }
}
