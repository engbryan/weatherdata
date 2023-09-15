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
    //[ApiController]
    public class AirportStatusController : ODataController
    {
        private readonly WeatherDataContext _context;
        private readonly AirportStatusManager _airportStatusManager;
        private readonly ILogger<AirportStatusController> _logger;

        public AirportStatusController(WeatherDataContext context, AirportStatusManager airportStatusManager, ILogger<AirportStatusController> logger)
        {
            _context = context;
            _airportStatusManager = airportStatusManager;
            _logger = logger;
        }

        /// <summary>
        /// Obtém dados meteorológicos atuais de um aeroporto a partir da integração Brasil API.
        /// </summary>
        /// <returns>O dado meteorológico.</returns>
        [HttpGet("GetAirportStatus/{icaoCode}/{asOf}")]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém condicoes climáticas de um aeroporto a partir da integração Brasil API")]
        public async Task<AirportStatus> GetAirportStatus([FromODataUri] string icaoCode, [FromODataUri] DateTime asOf)
            => await _airportStatusManager.EnsureFreshStatus(icaoCode, asOf);






        //Os métodos abaixo sao apenas para demonstrar o funcionamento das operacoes OData.
        //No cenário ideal, eles não deveriam utilizar o contexto do banco nesta camada.





        /// <summary>
        /// Obtém todos os dados meteorológicos em banco.
        /// </summary>
        /// <returns>Uma lista de dados meteorológicos.</returns>
        [HttpGet]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém todos os dados meteorológicos em banco")]
        public IQueryable<AirportStatus> GetAll()
            => _context.AirportStatus;

        /// <summary>
        /// Obtém um único dado meteorológico em banco pelo ID.
        /// </summary>
        /// <param name="codigo_icao">O ID do dado meteorológico.</param>
        /// <returns>O dado meteorológico correspondente ao ID.</returns>
        [HttpGet("{codigo_icao}")]
        [EnableQuery]
        [SwaggerOperation(Summary = "Obtém um único dado meteorológico em banco pelo ID")]
        [SwaggerResponse(200, "O dado meteorológico correspondente ao ID", typeof(AirportStatus))]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public SingleResult<AirportStatus> Get([FromODataUri] string codigo_icao)
        {
            IQueryable<AirportStatus> result = _context.AirportStatus.Where(w => w.codigo_icao == codigo_icao);
            return SingleResult.Create(result);
        }

        /// <summary>
        /// Cria um novo dado meteorológico em banco.
        /// </summary>
        /// <param name="airportStatus">O novo dado meteorológico a ser criado.</param>
        /// <returns>O dado meteorológico criado.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Cria um novo dado meteorológico em banco")]
        [SwaggerResponse(201, "O dado meteorológico criado", typeof(AirportStatus))]
        [SwaggerResponse(400, "Solicitação inválida")]
        public async Task<IActionResult> Post([FromBody] AirportStatus airportStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AirportStatus.Add(airportStatus);
            await _context.SaveChangesAsync();

            return Created(airportStatus);
        }

        /// <summary>
        /// Atualiza um dado meteorológico existente em banco pelo ID.
        /// </summary>
        /// <param name="codigoIcao">O ID do dado meteorológico.</param>
        /// <param name="airportStatus">Os dados meteorológicos atualizados.</param>
        /// <returns>Nenhum conteúdo ou BadRequest se ocorrer um erro.</returns>
        [HttpPut("{codigo_icao}")]
        [SwaggerOperation(Summary = "Atualiza um dado meteorológico existente em banco pelo ID")]
        [SwaggerResponse(204, "Dados meteorológicos atualizados com sucesso")]
        [SwaggerResponse(400, "Solicitação inválida")]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public async Task<IActionResult> Put([FromODataUri] string codigoIcao, [FromBody] AirportStatus airportStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (codigoIcao != airportStatus.codigo_icao)
            {
                return BadRequest();
            }

            _context.Entry(airportStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AirportStatus.Any(w => w.codigo_icao == codigoIcao))
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
        /// <param name="codigo_icao">O ID do dado meteorológico a ser excluído.</param>
        /// <returns>Nenhum conteúdo ou NotFound se o dado meteorológico não for encontrado.</returns>
        [HttpDelete("{codigo_icao}")]
        [SwaggerOperation(Summary = "Exclui um dado meteorológico em banco pelo ID")]
        [SwaggerResponse(204, "Dado meteorológico excluído com sucesso")]
        [SwaggerResponse(404, "Dado meteorológico não encontrado")]
        public async Task<IActionResult> Delete([FromODataUri] int codigo_icao)
        {
            AirportStatus airportStatus = await _context.AirportStatus.FindAsync(codigo_icao);

            if (airportStatus == null)
            {
                return NotFound();
            }

            _context.AirportStatus.Remove(airportStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
