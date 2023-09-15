using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

namespace WeatherData.Infrastructure.Logging
{
    public static class DbLoggerExtensions
    {
        public static ILoggingBuilder AddDbLogger(
            this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, DbLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <DbLoggerConfiguration, DbLoggerProvider>(builder.Services);

            builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider>();

            return builder;
        }
    }
}
