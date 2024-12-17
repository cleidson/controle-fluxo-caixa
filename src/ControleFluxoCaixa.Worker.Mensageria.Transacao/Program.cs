using ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria;
using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Core.Logic.Interfaces.Services;
using ControleFluxoCaixa.Core.Logic.Services;
using ControleFluxoCaixa.Infrastructure.Data;
using ControleFluxoCaixa.Infrastructure.Mensageria;
using ControleFluxoCaixa.Infrastructure.Respositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ControleFluxoCaixa.Worker.Mensageria.Transacao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Configuração do RabbitMQ
                    var rabbitMqConfig = context.Configuration.GetSection("RabbitMQ");
                    services.AddSingleton<IMessageQueueService>(sp =>
                        new RabbitMQService(
                            hostname: rabbitMqConfig["HostName"]
                                      ?? throw new ArgumentNullException(nameof(rabbitMqConfig), "HostName is not configured."),
                            username: rabbitMqConfig["UserName"]
                                      ?? throw new ArgumentNullException(nameof(rabbitMqConfig), "UserName is not configured."),
                            password: rabbitMqConfig["Password"]
                                      ?? throw new ArgumentNullException(nameof(rabbitMqConfig), "Password is not configured.")
                        ));

                    // Registro do ControleFluxoCaixaDbContext (Banco Primário)
                    services.AddDbContext<ControleFluxoCaixaDbContext>(options =>
                        options.UseSqlServer(
                            context.Configuration.GetConnectionString("PrincipalDB"),
                            sqlOptions =>
                            {
                                sqlOptions.EnableRetryOnFailure();
                                sqlOptions.CommandTimeout(60);
                                sqlOptions.MaxBatchSize(100);
                            }));

              
                    // Registro dos repositórios genéricos
                    services.AddScoped(typeof(ITransactionalRepository<>), typeof(TransactionalRepository<>));
                    services.AddScoped(typeof(IReadOnlyDataRepository<>), typeof(ReadOnlyDataRepository<>));

                    // Registro dos serviços de domínio
                    services.AddScoped<ITransacaoService, TransacaoService>();

                    // Registro do Worker
                    services.AddHostedService<Worker>();
                })
                .Build();
            host.Run();
        }
    }
}
