using ControleFluxoCaixa.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.Data.Configurations
{
    public class SaldoDiarioConfiguration : IEntityTypeConfiguration<SaldoDiario>
    {
        public void Configure(EntityTypeBuilder<SaldoDiario> builder)
        {
            builder.ToTable("SaldosDiarios");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.UsuarioId)
                   .IsRequired();

            builder.Property(s => s.SaldoAtual)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(s => s.DataHora)
                   .IsRequired()
                   .HasConversion(
                       v => v.ToUniversalTime(),     // Ao salvar, converte para UTC
                       v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Ao ler, define como UTC
                   );

        }
    }
}
