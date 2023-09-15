using System.Threading.Tasks;
using WeatherData.Models;

namespace WeatherData.Services.Interfaces
{
    public interface IWeatherDataService
    {
        Task<WeatherForecast> GetCityWeatherAsync(ushort cityCode, ushort days);
        Task<AirportStatus> GetAirportStatusAsync(string icaoCode);
    }
}
