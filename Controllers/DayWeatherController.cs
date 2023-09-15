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
    public class DayWeatherController : ODataController
    {
        private readonly WeatherDataContext _context;
        private readonly WeatherDataManager _weatherDataManager;
        private readonly ILogger<DayWeatherController> _logger;
        private readonly IWeatherDataService _weatherService;

        public DayWeatherController(WeatherDataContext context, WeatherDataManager weatherDataManager, ILogger<DayWeatherController> logger)
        {
            _context = context;
            _weatherDataManager = weatherDataManager;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os dados meteorológicos em banco.
        /// </summary>
        /// <returns>Uma lista de dados meteorológicos.</returns>
        [HttpGet]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém todos os dados meteorológicos em banco")]
        public IQueryable<DayWeather> GetAll()
        {
            return _context.DayWeather;
        }

        /// <summary>
        /// Obtém um único dado meteorológico em banco pelo ID.
        /// </summary>
        /// <param name="id">O ID do dado meteorológico.</param>
        /// <returns>O dado meteorológico correspondente ao ID.</returns>
        [HttpGet("{id}")]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém um único dado meteorológico em banco pelo ID")]
        [SwaggerResponse(200, "O dado meteorológico correspondente ao ID", typeof(DayWeather))]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public SingleResult<DayWeather> Get([FromODataUri] int id)
        {
            IQueryable<DayWeather> result = _context.DayWeather.Where(w => w.id == id);
            return SingleResult.Create(result);
        }

        /// <summary>
        /// Cria um novo dado meteorológico em banco.
        /// </summary>
        /// <param name="dayWeather">O novo dado meteorológico a ser criado.</param>
        /// <returns>O dado meteorológico criado.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo dado meteorológico em banco")]
        [SwaggerResponse(201, "O dado meteorológico criado", typeof(DayWeather))]
        [SwaggerResponse(400, "Solicitação inválida")]
        public async Task<IActionResult> Post([FromBody] DayWeather dayWeather)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DayWeather.Add(dayWeather);
            await _context.SaveChangesAsync();

            return Created(dayWeather);
        }

        /// <summary>
        /// Atualiza um dado meteorológico existente em banco pelo ID.
        /// </summary>
        /// <param name="id">O ID do dado meteorológico.</param>
        /// <param name="dayWeather">Os dados meteorológicos atualizados.</param>
        /// <returns>Nenhum conteúdo ou BadRequest se ocorrer um erro.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza um dado meteorológico existente em banco pelo ID")]
        [SwaggerResponse(204, "Dados meteorológicos atualizados com sucesso")]
        [SwaggerResponse(400, "Solicitação inválida")]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public async Task<IActionResult> Put([FromODataUri] int id, [FromBody] DayWeather dayWeather)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dayWeather.id)
            {
                return BadRequest();
            }

            _context.Entry(dayWeather).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DayWeather.Any(w => w.id == id))
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
        /// <param name="id">O ID do dado meteorológico a ser excluído.</param>
        /// <returns>Nenhum conteúdo ou NotFound se o dado meteorológico não for encontrado.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui um dado meteorológico em banco pelo ID")]
        [SwaggerResponse(204, "Dado meteorológico excluído com sucesso")]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public async Task<IActionResult> Delete([FromODataUri] int id)
        {
            DayWeather dayWeather = await _context.DayWeather.FindAsync(id);

            if (dayWeather == null)
            {
                return NotFound();
            }

            _context.Remove(dayWeather);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
