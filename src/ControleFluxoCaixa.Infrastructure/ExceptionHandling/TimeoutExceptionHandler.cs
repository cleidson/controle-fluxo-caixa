using ControleFluxoCaixa.Core.Logic.Interfaces.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.ExceptionHandling
{
    public class TimeoutExceptionHandler : IExceptionHandler
    {
        public async Task<bool> HandleAsync(Exception exception, HttpContext context)
        {
            if (exception is TimeoutException)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("O serviço de mensageria está indisponível. Tente novamente mais tarde.");
                return true;
            }
            return false; // Deixa o próximo handler tentar
        }
    }
}
