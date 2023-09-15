using Microsoft.EntityFrameworkCore;
using WeatherData.Models;

namespace WeatherData.DataAccess
{
    public class WeatherDataContext : DbContext
    {
        public WeatherDataContext(DbContextOptions<WeatherDataContext> options) : base(options) { }
         
        public DbSet<WeatherForecast> WeatherForecast { get; set; }
        public DbSet<DayWeather> DayWeather { get; set; }
        public DbSet<AirportStatus> AirportStatus { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}
