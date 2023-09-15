using Microsoft.AspNetCore.Http.Extensions;
using System.Reflection;
using System.Security.Cryptography;
using WeatherData.DataAccess;
using WeatherData.Models;

namespace WeatherData.Infrastructure.ExceptionHandling
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //executa qualquer requisicao
                await _next(context);
            }
            catch (Exception ex)
            {
                //loga no banco
                _logger.LogError(ex, $"Requisição falhou.");

                //retorna 500 ao usuário
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                //mostra no console do serviço
                Console.Error.WriteLine("Ocorreu um erro interno:");
                Console.Error.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.Error.WriteLine($"Mensagem: {ex.Message}");
                Console.Error.WriteLine($"Rastreamento de pilha:\n{ex.StackTrace}");
            }
        }
    }
}
