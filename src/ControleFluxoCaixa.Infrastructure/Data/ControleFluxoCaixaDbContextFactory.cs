using ControleFluxoCaixa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ControleFluxoCaixa.Infrastructure
{
    public class ControleFluxoCaixaDbContextFactory : IDesignTimeDbContextFactory<ControleFluxoCaixaDbContext>
    {
        public ControleFluxoCaixaDbContext CreateDbContext(string[] args)
        {
            // Carregar a configuração do appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName + "/ControleFluxoCaixa.Api")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configurar o DbContextOptions para PostgreSQL
            var optionsBuilder = new DbContextOptionsBuilder<ControleFluxoCaixaDbContext>();
            var connectionString = configuration.GetConnectionString("PrincipalDB");

            optionsBuilder.UseNpgsql(connectionString);

            // Retornar a instância do DbContext
            return new ControleFluxoCaixaDbContext(optionsBuilder.Options);
        }
    }
}
