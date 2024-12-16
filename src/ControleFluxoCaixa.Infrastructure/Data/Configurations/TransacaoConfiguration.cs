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
    public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.ToTable("Transacoes");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.UsuarioId)
                   .IsRequired();

            builder.Property(t => t.Valor)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.DataHora)
                   .IsRequired();

            builder.Property(t => t.Descricao)
                   .HasMaxLength(250);

            builder.Property(t => t.Tipo)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}
