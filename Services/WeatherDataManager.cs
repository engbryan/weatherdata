using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeatherData.DataAccess;
using WeatherData.Models;
using WeatherData.Services.Interfaces;
using Z.EntityFramework.Plus;

namespace WeatherData.Services
{
    public class WeatherDataManager
    {
        private readonly WeatherDataContext _dbContext;
        private readonly IWeatherDataService _weatherService;

        public WeatherDataManager(WeatherDataContext dbContext, IWeatherDataService weatherService)
        {
            _dbContext = dbContext;
            _weatherService = weatherService;
        }

        public async Task<WeatherForecast> EnsureFreshForecast(ushort codCidade, DateTime forecastDate)
        {
            forecastDate = forecastDate.Date;

            WeatherForecast? weatherData = null;

            //Se data for anterior a hoje, busca do banco
            if (forecastDate < DateTime.Today.Date)
            {
                forecastDate = DateTime.Today.Date;

                weatherData = await GetSavedForecasts(codCidade, forecastDate);
            }

            //Verifica se a data tem dias a frente
            var daysAmmount = (forecastDate - DateTime.Today.Date).Days;

            if (daysAmmount >= 0)
            {
                // Busca os dados do serviço
                weatherData = await GetForecasts(codCidade, daysAmmount);

                // Atualiza o banco de dados com os novos dados climáticos
                await SaveForecasts(weatherData!);
            }

            return weatherData!;
        }

        private async Task<WeatherForecast?> GetForecasts(ushort codCidade, int daysAmmount)
        {
            var weatherData = await _weatherService.GetCityWeatherAsync(codCidade, (ushort)daysAmmount);

            if (weatherData == null)
            {
                throw new Exception("Erro ao obter dados climáticos do serviço.");
            }

            return weatherData;
        }

        private async Task SaveForecasts(WeatherForecast weatherData)
        {
            _dbContext.WeatherForecast.Update(weatherData);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<WeatherForecast?> GetSavedForecasts(ushort codCidade, DateTime forecastDate)
        {
            var weatherData = await _dbContext.WeatherForecast
                .Where(w => w.codCidade == codCidade)
                .IncludeFilter(w => w.clima
                    .Where(c => c.data == forecastDate)
                    )
                .FirstOrDefaultAsync();

            return weatherData;
        }
    }
}