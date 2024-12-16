using ControleFluxoCaixa.Core.Logic.Interfaces.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Infrastructure.ExceptionHandling
{
    public class GenericExceptionHandler : IExceptionHandler
    {
        public async Task<bool> HandleAsync(Exception exception, HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Ocorreu um erro inesperado ao processar sua solicitação.");
            return true;
        }
    }
}
