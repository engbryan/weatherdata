using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeatherData.DataAccess;
using WeatherData.Models;
using WeatherData.Services.Interfaces;
using Z.EntityFramework.Plus;

namespace WeatherData.Services
{
    public class AirportStatusManager
    {
        private readonly WeatherDataContext _dbContext;
        private readonly IWeatherDataService _weatherService;

        public AirportStatusManager(WeatherDataContext dbContext, IWeatherDataService weatherService)
        {
            _dbContext = dbContext;
            _weatherService = weatherService;
        }

        public async Task<AirportStatus> EnsureFreshStatus(string codigoIcao)
        {
            return await EnsureFreshStatus(codigoIcao, DateTime.Today);
        }

        public async Task<AirportStatus> EnsureFreshStatus(string codigoIcao, DateTime asOf)
        {
            asOf = asOf.Date;

            AirportStatus? airportStatus = null;

            //Se data for anterior a hoje, busca do banco
            if (asOf < DateTime.Today)
            {
                asOf = DateTime.Today;

                airportStatus = await GetSavedStatus(codigoIcao, asOf);
            }
            else
            {
                // Busca os dados do serviço
                airportStatus = await GetCurrentStatus(codigoIcao);

                // Atualiza o banco de dados com os novos dados climáticos
                await SaveStatus(airportStatus);
            }

            return airportStatus!;
        }

        private async Task<AirportStatus> GetCurrentStatus(string codigoIcao)
        {
            var airportStatus = await _weatherService.GetAirportStatusAsync(codigoIcao);

            if (airportStatus == null)
            {
                throw new Exception("Erro ao obter dados climáticos do serviço.");
            }

            return airportStatus;
        }

        private async Task SaveStatus(AirportStatus airportStatus)
        {
            _dbContext.AirportStatus.Update(airportStatus);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<AirportStatus?> GetSavedStatus(string codigoIcao, DateTime asOf)
        {
            var airportStatus = await _dbContext.AirportStatus
                .Where(w => w.codigo_icao == codigoIcao && w.atualizado_em.Date == asOf)
                .FirstOrDefaultAsync();

            return airportStatus;
        }
    }
}