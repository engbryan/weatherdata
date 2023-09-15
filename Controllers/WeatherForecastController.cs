using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WeatherData.DataAccess;
using WeatherData.Models;
using WeatherData.Services;
using WeatherData.Services.Interfaces;

namespace WeatherData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class WeatherForecastController : ODataController
    {
        private readonly WeatherDataContext _context;
        private readonly WeatherDataManager _weatherDataManager;
        private readonly ILogger<DayWeatherController> _logger;
        private readonly IWeatherDataService _weatherService;

        public WeatherForecastController(WeatherDataContext context, WeatherDataManager weatherDataManager, ILogger<DayWeatherController> logger)
        {
            _context = context;
            _weatherDataManager = weatherDataManager;
            _logger = logger;
        }

        /// <summary>
        /// Obtém dados meteorológicos para uma cidade a partir da integração Brasil API.
        /// </summary>
        /// <returns>O dado meteorológico.</returns>
        [HttpGet("GetCityWeatherAsOf/{cityCode}/{forecastDate}")]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém dados meteorológicos para uma cidade a partir da integração Brasil API")]
        public async Task<WeatherForecast> GetCityWeatherAsOf([FromODataUri] ushort cityCode, [FromODataUri] DateTime forecastDate)
            => await _weatherDataManager.EnsureFreshForecast(cityCode, forecastDate);





        //Os métodos abaixo sao apenas para demonstrar o funcionamento das operacoes OData.
        //No cenário ideal, eles não deveriam utilizar o contexto do banco nesta camada.




        /// <summary>
        /// Obtém todos os dados meteorológicos em banco.
        /// </summary>
        /// <returns>Uma lista de dados meteorológicos.</returns>
        [HttpGet]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém todos os dados meteorológicos em banco")]
        public IQueryable<WeatherForecast> GetAll()
            => _context.WeatherForecast;

        /// <summary>
        /// Obtém um único dado meteorológico em banco pelo ID.
        /// </summary>
        /// <param name="codCidade">O ID do dado meteorológico.</param>
        /// <returns>O dado meteorológico correspondente ao ID.</returns>
        [HttpGet("{codCidade}")]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém um único dado meteorológico em banco pelo ID")]
        [SwaggerResponse(200, "O dado meteorológico correspondente ao ID", typeof(WeatherForecast))]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public IQueryable<WeatherForecast> Get([FromODataUri] int codCidade)
            => _context.WeatherForecast.Where(w => w.codCidade == codCidade);


        /// <summary>
        /// Cria um novo dado meteorológico em banco.
        /// </summary>
        /// <param name="weatherForecast">O novo dado meteorológico a ser criado.</param>
        /// <returns>O dado meteorológico criado.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo dado meteorológico em banco")]
        [SwaggerResponse(201, "O dado meteorológico criado", typeof(WeatherForecast))]
        [SwaggerResponse(400, "Solicitação inválida")]
        public async Task<IActionResult> Post([FromBody] WeatherForecast weatherForecast)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WeatherForecast.Add(weatherForecast);
            await _context.SaveChangesAsync();

            return Created(weatherForecast);
        }

        /// <summary>
        /// Atualiza um dado meteorológico existente em banco pelo ID.
        /// </summary>
        /// <param name="codCidade">O ID do dado meteorológico.</param>
        /// <param name="weatherForecast">Os dados meteorológicos atualizados.</param>
        /// <returns>Nenhum conteúdo ou BadRequest se ocorrer um erro.</returns>
        [HttpPut("{codCidade}")]
        [SwaggerOperation(Summary = "Atualiza um dado meteorológico existente em banco pelo ID")]
        [SwaggerResponse(204, "Dados meteorológicos atualizados com sucesso")]
        [SwaggerResponse(400, "Solicitação inválida")]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public async Task<IActionResult> Put([FromODataUri] int codCidade, [FromBody] WeatherForecast weatherForecast)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (codCidade != weatherForecast.codCidade)
            {
                return BadRequest();
            }

            _context.Entry(weatherForecast).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.WeatherForecast.Any(w => w.codCidade == codCidade))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Exclui um dado meteorológico em banco pelo ID.
        /// </summary>
        /// <param name="codCidade">O ID do dado meteorológico a ser excluído.</param>
        /// <returns>Nenhum conteúdo ou NotFound se o dado meteorológico não for encontrado.</returns>
        [HttpDelete("{codCidade}")]
        [SwaggerOperation(Summary = "Exclui um dado meteorológico em banco pelo ID")]
        [SwaggerResponse(204, "Dado meteorológico excluído com sucesso")]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public async Task<IActionResult> Delete([FromODataUri] int codCidade)
        {
            WeatherForecast weatherForecast = await _context.WeatherForecast.FindAsync(codCidade);

            if (weatherForecast == null)
            {
                return NotFound();
            }

            _context.Remove(weatherForecast);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
