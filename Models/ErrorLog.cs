namespace WeatherData.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string LogLevel { get; set; }
    }
}
