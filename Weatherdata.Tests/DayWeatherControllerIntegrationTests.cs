using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherData.DataAccess;
using WeatherData.Models;
using Xunit;

namespace WeatherData.Tests.Integration
{
    public class DayWeatherIntegrationTests : IClassFixture<WebApplicationFactory<WeatherData.Startup>>
    {
        private readonly WebApplicationFactory<WeatherData.Startup> _factory;

        public DayWeatherIntegrationTests(WebApplicationFactory<WeatherData.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateDayWeather_ValidData_ReturnsCreatedDayWeather()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newDayWeather = new
            {
                data = DateTime.Now,
                condicao = "Sunny",
                min = 20,
                max = 30,
                indice_uv = 5,
                condicao_desc = "Clear skies"
            };

            // Act
            var response = await client.PostAsync("/odata/DayWeather", new StringContent(
                JsonConvert.SerializeObject(newDayWeather),
                Encoding.UTF8,
                "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Verifique se os dados foram adicionados ao banco de dados
            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
                var addedDayWeather = dbContext.DayWeather.FirstOrDefault(d => d.condicao == "Sunny");

                Assert.NotNull(addedDayWeather);
                Assert.Equal(newDayWeather.data.Date, addedDayWeather.data.Date);
                Assert.Equal(newDayWeather.min, addedDayWeather.min);
                Assert.Equal(newDayWeather.max, addedDayWeather.max);
            }
        }

        [Fact]
        public async Task GetDayWeatherById_ExistingId_ReturnsDayWeather()
        {
            // Arrange
            var client = _factory.CreateClient();
            var existingId = 1; // Substitua pelo ID real do DayWeather existente em seu banco

            // Act
            var response = await client.GetAsync($"/odata/DayWeather/{existingId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var dayWeather = JsonConvert.DeserializeObject<DayWeather>(content);

            Assert.NotNull(dayWeather);
            Assert.Equal(existingId, dayWeather.id);
        }

        [Fact]
        public async Task UpdateDayWeather_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var existingId = 1; // Substitua pelo ID real do DayWeather existente em seu banco
            var updatedDayWeather = new
            {
                data = DateTime.Now,
                condicao = "Rainy",
                min = 15,
                max = 25,
                indice_uv = 3,
                condicao_desc = "Heavy rain"
            };

            // Act
            var response = await client.PutAsync($"/odata/DayWeather/{existingId}", new StringContent(
                JsonConvert.SerializeObject(updatedDayWeather),
                Encoding.UTF8,
                "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verifique se os dados foram atualizados no banco de dados
            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
                var updatedDayWeatherEntity = dbContext.DayWeather.FirstOrDefault(d => d.id == existingId);

                Assert.NotNull(updatedDayWeatherEntity);
                Assert.Equal(updatedDayWeather.condicao, updatedDayWeatherEntity.condicao);
                Assert.Equal(updatedDayWeather.min, updatedDayWeatherEntity.min);
                Assert.Equal(updatedDayWeather.max, updatedDayWeatherEntity.max);
            }
        }

        [Fact]
        public async Task DeleteDayWeather_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var existingId = 1; // Substitua pelo ID real do DayWeather existente em seu banco

            // Act
            var response = await client.DeleteAsync($"/odata/DayWeather/{existingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verifique se os dados foram removidos do banco de dados
            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
                var deletedDayWeatherEntity = dbContext.DayWeather.FirstOrDefault(d => d.id == existingId);

                Assert.Null(deletedDayWeatherEntity);
            }
        }

    }
}
