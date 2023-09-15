using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using WeatherData;
using WeatherData.DataAccess;
using WeatherData.Models;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherData.Tests.Integration
{
    public class WeatherDataIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WeatherDataIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetWeatherDataById_ReturnsWeatherData()
        {
            // Arrange
            var client = _factory.CreateClient();
            int cityId = 4750;

            // Act
            var response = await client.GetAsync($"/odata/WeatherData/{cityId}");
            var content = await response.Content.ReadAsStringAsync();
            var returnedWeatherData = JsonConvert.DeserializeObject<WeatherForecast>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(returnedWeatherData);
            Assert.Equal(cityId, returnedWeatherData.codCidade);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbWeatherData = await context.WeatherForecast.FindAsync(cityId);
            Assert.NotNull(dbWeatherData);
        }

        [Fact]
        public async Task CreateWeatherData_ReturnsCreatedResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            int cityCode = 4750;

            var newWeatherData = new WeatherForecast
            {
                codCidade = cityCode,
                cidade = "City4750",
                estado = "State4750",
                atualizado_em = DateTime.Now.ToString(),
                clima = new System.Collections.Generic.List<DayWeather>
                {
                    new DayWeather
                    {
                        data = DateTime.Now.Date,
                        condicao = "Sunny",
                        min = 20,
                        max = 30,
                        indice_uv = 5,
                        condicao_desc = "Clear skies"
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(newWeatherData), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/odata/WeatherData", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbWeatherData = await context.WeatherForecast.FindAsync(cityCode);
            Assert.NotNull(dbWeatherData);
        }

        [Fact]
        public async Task UpdateWeatherData_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            int cityId = 4750;

            var updatedWeatherData = new WeatherForecast
            {
                codCidade = cityId,
                cidade = "City4750",
                estado = "State4750",
                atualizado_em = DateTime.Now.ToString(),
                clima = new System.Collections.Generic.List<DayWeather>
                {
                    new DayWeather
                    {
                        data = DateTime.Now.Date,
                        condicao = "Sunny",
                        min = 20,
                        max = 30,
                        indice_uv = 5,
                        condicao_desc = "Clear skies"
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedWeatherData), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"/odata/WeatherData/{cityId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbWeatherData = await context.WeatherForecast.FindAsync(cityId);
            Assert.NotNull(dbWeatherData);
        }

        [Fact]
        public async Task DeleteWeatherData_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            int cityId = 4750;

            // Act
            var response = await client.DeleteAsync($"/odata/WeatherData/{cityId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbWeatherData = await context.WeatherForecast.FindAsync(cityId);
            Assert.Null(dbWeatherData); // Deve ser nulo após a exclusão
        }
    }
}
