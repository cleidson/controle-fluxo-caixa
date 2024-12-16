using ControleFluxoCaixa.Core.Logic.Interfaces.Mensageria;
using ControleFluxoCaixa.Infrastructure.Mensageria;
using ControleFluxoCaixa.Infrastructure.ExceptionHandling;
using Microsoft.OpenApi.Models;
using ControleFluxoCaixa.Api.Middlewares;
using ControleFluxoCaixa.Core.Logic.Interfaces.ExceptionHandler;
using ControleFluxoCaixa.Core.Logic.Interfaces.Repository;
using ControleFluxoCaixa.Infrastructure.Respositories;
using ControleFluxoCaixa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ControleFluxoCaixa.Api
{
    public class Program
    {
       
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicionar servi�os ao cont�iner
            builder.Services.AddControllers();

            // Configurar Swagger/OpenAPI
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Controle Fluxo de Caixa API",
                    Version = "v1"
                });
            });

            // Configurar CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", corsBuilder =>
                {
                    corsBuilder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                });
            });

            // Registrar o servi�o de mensageria (RabbitMQ)
            builder.Services.AddSingleton<IMessageQueueService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var rabbitMqConfig = configuration.GetSection("RabbitMQ");

                return new RabbitMQService(
                    hostname: rabbitMqConfig["HostName"]
                              ?? throw new ArgumentNullException(nameof(rabbitMqConfig), "HostName is not configured."),
                    username: rabbitMqConfig["UserName"]
                              ?? throw new ArgumentNullException(nameof(rabbitMqConfig), "UserName is not configured."),
                    password: rabbitMqConfig["Password"]
                              ?? throw new ArgumentNullException(nameof(rabbitMqConfig), "Password is not configured.")
                );
            });

            // Registrar os handlers no cont�iner de DI
            builder.Services.AddScoped<IExceptionHandler, TimeoutExceptionHandler>();
            builder.Services.AddScoped<IExceptionHandler, InvalidOperationExceptionHandler>();
            builder.Services.AddScoped<IExceptionHandler, GenericExceptionHandler>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Registro do ControleFluxoCaixaDbContext (Banco Prim�rio)
            builder.Services.AddDbContext<ControleFluxoCaixaDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("PrincipalDB"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                        sqlOptions.CommandTimeout(60);
                        sqlOptions.MaxBatchSize(100);
                    }));

            // Registro do ControleFluxoCaixaReadOnlyDbContext (Banco Secund�rio - R�plica)
            builder.Services.AddDbContext<ControleFluxoCaixaReadOnlyDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("ReplicaDB"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                        sqlOptions.CommandTimeout(60);
                    })
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            var app = builder.Build();

            // Aplicar migrations automaticamente no banco prim�rio (apenas em dev)
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                { 
                    var dbContext = scope.ServiceProvider.GetRequiredService<ControleFluxoCaixaDbContext>();
                    dbContext.Database.Migrate();

                    // Validar conex�o com o banco secund�rio
                    var readOnlyDbContext = scope.ServiceProvider.GetRequiredService<ControleFluxoCaixaReadOnlyDbContext>();
                    if (readOnlyDbContext.Database.CanConnect())
                    {
                        Console.WriteLine("Conex�o com o banco de leitura estabelecida com sucesso.");
                    }
                }
            }

            // Configura��es do pipeline HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer
                            {
                                Url = $"{httpReq.Scheme}://{httpReq.Host}"
                            }
                        };
                    });
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Controle Fluxo de Caixa API v1");
                });
            }

            // Configurar CORS
            app.UseCors("AllowAllOrigins");

            // Configurar HTTPS
            app.UseHttpsRedirection();

            // Configurar autoriza��o
            app.UseAuthorization();

            // Configurar o middleware de exce��es
            app.UseMiddleware<ExceptionMiddleware>();

            // Mapear os controllers
            app.MapControllers();

            // Log de requisi��es (opcional)
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[{DateTime.Now}] Request: {context.Request.Method} {context.Request.Path}");
                await next.Invoke();
            });

            // Iniciar o aplicativo
            app.Run();
        }
    }
}
