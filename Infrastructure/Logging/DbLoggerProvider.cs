using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using WeatherData.DataAccess;
using WeatherData.Models;

namespace WeatherData.Infrastructure.Logging
{
    [ProviderAlias("DbLogger")]
    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private readonly ConcurrentDictionary<string, DbLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        public readonly IConfiguration _configuration;

        private DbLoggerConfiguration _currentConfig;

        public DbLoggerProvider(IOptionsMonitor<DbLoggerConfiguration> config, IConfiguration configuration)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig =>
            _currentConfig = updatedConfig);
            _configuration = configuration;
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new DbLogger(name, GetCurrentConfig, _configuration));

        private DbLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
