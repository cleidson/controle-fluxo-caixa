
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace ControleFluxoCaixa.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicionar serviços ao contêiner
            builder.Services.AddControllers();

            // Adicionar suporte ao Swagger/OpenAPI
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

            var app = builder.Build();

            // Aplicar CORS
            app.UseCors("AllowAllOrigins");

            // Configurar o pipeline HTTP
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

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
