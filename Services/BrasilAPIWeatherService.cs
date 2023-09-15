using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherData.Models;
using WeatherData.Services.Interfaces;

namespace WeatherData.Services
{
    public class BrasilAPIWeatherService : IWeatherDataService
    {
        private readonly HttpClient _httpClient;

        public BrasilAPIWeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://brasilapi.com.br/api/cptec/v1/clima/previsao/{cityCode}/{days}");
        }

        public async Task<WeatherForecast> GetCityWeatherAsync(ushort cityCode, ushort days)
        {
            if (days > 6) days = 6;
            if (days == 0) days = 1;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/cptec/v1/clima/previsao/{cityCode}/{days}");

                if (response.IsSuccessStatusCode)
                {
                    var weatherJson = await response.Content.ReadAsStringAsync();
                    var weatherResponse = JsonConvert.DeserializeObject<WeatherForecast>(weatherJson)!;
                    
                    weatherResponse.codCidade = cityCode;

                    return weatherResponse;
                }
                else
                {
                    throw new HttpRequestException($"Erro ao obter dados de clima. Código de status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                throw; 
            }
        }

        public async Task<AirportStatus> GetAirportStatusAsync(string icaoCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/cptec/v1/clima/aeroporto/{icaoCode}");

                if (response.IsSuccessStatusCode)
                {
                    var weatherJson = await response.Content.ReadAsStringAsync();
                    var airportResponse = JsonConvert.DeserializeObject<AirportStatus>(weatherJson)!;

                    return airportResponse;
                }
                else
                {
                    throw new HttpRequestException($"Erro ao obter dados de aeroporto. Código de status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }
    }
}
