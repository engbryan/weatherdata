namespace WeatherData.Infrastructure.Logging
{
    public class DbLoggerConfiguration

    {
        public int EventId { get; set; }

        public List<LogLevel> LogLevels { get; set; } = new()
        {
            LogLevel.Information
        };
    }
}
