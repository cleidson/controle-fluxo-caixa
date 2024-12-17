using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace ControleFluxoCaixa.Infrastructure.Data
{
    public class ControleFluxoCaixaDbContextFactory : IDesignTimeDbContextFactory<ControleFluxoCaixaDbContext>
    {
        public ControleFluxoCaixaDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName + "/ControleFluxoCaixa.Api") // Ajuste para apontar para o projeto API
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ControleFluxoCaixaDbContext>();
            var connectionString = configuration.GetConnectionString("PrincipalDB");

            optionsBuilder.UseSqlServer(connectionString);

            return new ControleFluxoCaixaDbContext(optionsBuilder.Options);
        }
    }
}

