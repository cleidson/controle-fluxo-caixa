using ControleFluxoCaixa.Core.Logic.Interfaces.ExceptionHandler;

namespace ControleFluxoCaixa.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExceptionMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Criar um escopo para resolver os handlers Scoped
                using var scope = _scopeFactory.CreateScope();
                var handlers = scope.ServiceProvider.GetServices<IExceptionHandler>();

                foreach (var handler in handlers)
                {
                    if (await handler.HandleAsync(ex, context))
                    {
                        return;
                    }
                }

                // Fallback para exceções não tratadas
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Erro não tratado.");
            }
        }
    }

}
