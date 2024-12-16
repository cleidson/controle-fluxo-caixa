using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ControleFluxoCaixa.Core.Logic.Interfaces.ExceptionHandler
{
    public interface IExceptionHandler
    {
        Task<bool> HandleAsync(Exception exception, HttpContext context);
    }
}
