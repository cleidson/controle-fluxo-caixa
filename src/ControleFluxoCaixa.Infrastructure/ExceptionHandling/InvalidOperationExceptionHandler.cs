using ControleFluxoCaixa.Core.Logic.Interfaces.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.ExceptionHandling
{
    public class InvalidOperationExceptionHandler : IExceptionHandler
    {
        public async Task<bool> HandleAsync(Exception exception, HttpContext context)
        {
            if (exception is InvalidOperationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(exception.Message); // Retorna a mensagem da exceção
                return true;
            }
            return false;
        }
    }
}
