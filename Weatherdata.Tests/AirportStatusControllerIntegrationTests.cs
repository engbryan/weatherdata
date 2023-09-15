using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using WeatherData;
using WeatherData.DataAccess;
using WeatherData.Models;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherData.Tests.Integration
{
    public class AirportStatusIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public AirportStatusIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAirportStatus_ReturnsAirportStatus()
        {
            // Arrange
            var client = _factory.CreateClient();
            string icaoCode = "SBAR";
            DateTime asOf = DateTime.Now.AddDays(3);

            // Act
            var response = await client.GetAsync($"/odata/AirportStatus/GetAirportStatus/{icaoCode}/{asOf}");
            var content = await response.Content.ReadAsStringAsync();
            var returnedAirportStatus = JsonConvert.DeserializeObject<AirportStatus>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(returnedAirportStatus);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbAirportStatus = await context.AirportStatus.FindAsync(icaoCode);
            Assert.NotNull(dbAirportStatus);
        }

        [Fact]
        public async Task CreateAirportStatus_ReturnsCreatedResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            string icaoCode = "SBAR";

            var newAirportStatus = new AirportStatus
            {
                codigo_icao = icaoCode,
                atualizado_em = DateTime.Now,
                pressao_atmosferica = "1013 hPa",
                visibilidade = "10 km",
                vento = 10,
                direcao_vento = 90,
                umidade = 50,
                condicao = "Ensolarado",
                condicao_Desc = "Céu limpo",
                temp = 25
            };

            var content = new StringContent(JsonConvert.SerializeObject(newAirportStatus), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/odata/AirportStatus", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbAirportStatus = await context.AirportStatus.FindAsync(icaoCode);
            Assert.NotNull(dbAirportStatus);
        }

        [Fact]
        public async Task UpdateAirportStatus_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            string icaoCode = "SBAR";

            var updatedAirportStatus = new AirportStatus
            {
                codigo_icao = icaoCode,
                atualizado_em = DateTime.Now,
                pressao_atmosferica = "1025 hPa",
                visibilidade = "2 km",
                vento = 15,
                direcao_vento = 120,
                umidade = 55,
                condicao = "Nublado",
                condicao_Desc = "Céu parcialmente nublado",
                temp = 20
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedAirportStatus), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"/odata/AirportStatus/{icaoCode}", content);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbAirportStatus = await context.AirportStatus.FindAsync(icaoCode);
            Assert.NotNull(dbAirportStatus);
            Assert.Equal(updatedAirportStatus.codigo_icao, dbAirportStatus.codigo_icao);
            Assert.Equal(updatedAirportStatus.pressao_atmosferica, dbAirportStatus.pressao_atmosferica);
            Assert.Equal(updatedAirportStatus.visibilidade, dbAirportStatus.visibilidade);

        }

        [Fact]
        public async Task DeleteAirportStatus_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            string icaoCode = "SBAR";

            // Act
            var response = await client.DeleteAsync($"/odata/AirportStatus/{icaoCode}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verifique se o banco de dados foi atualizado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
            var dbAirportStatus = await context.AirportStatus.FindAsync(icaoCode);
            Assert.Null(dbAirportStatus); // Deve ser nulo após a exclusão
        }
    }
}
