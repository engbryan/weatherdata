using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Extensions;
using WeatherData.DataAccess;
using WeatherData.Models;

namespace WeatherData.Infrastructure.Logging
{
    public class DbLogger : ILogger
    {
        private readonly string _name;
        private readonly Func<DbLoggerConfiguration> _getCurrentConfig;
        private readonly WeatherDataContext _dbContext;

        public DbLogger(
            string name,
            Func<DbLoggerConfiguration> getCurrentConfig,
            IConfiguration configuration
            )
        {
            (_name, _getCurrentConfig) = (name, getCurrentConfig);

            var options = new DbContextOptionsBuilder<WeatherDataContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;

            _dbContext = new WeatherDataContext(options);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => 
            _getCurrentConfig().LogLevels.Contains(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                var innerMostException = exception?.GetBaseException();

                if (innerMostException != null)
                {
                    var errorLog = new ErrorLog
                    {
                        Timestamp = DateTime.UtcNow,
                        ExceptionType = innerMostException.GetType().FullName!,
                        Message = innerMostException.Message,
                        StackTrace = innerMostException.StackTrace!,
                        LogLevel = logLevel.GetDisplayName()!

                    };

                    _dbContext.ErrorLogs.Add(errorLog);
                    _dbContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Ocorreu um erro com o logger:");
                Console.Error.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.Error.WriteLine($"Mensagem: {ex.Message}");
                Console.Error.WriteLine($"Rastreamento de pilha:\n{ex.StackTrace}");
            }
        }
    }
}
