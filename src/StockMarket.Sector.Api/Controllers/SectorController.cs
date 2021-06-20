using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Sector.Api.Entities;
using StockMarket.Sector.Api.Services;

namespace StockMarket.Sector.Api.Controllers
{
    [ApiController]
    [Route("sectors")]
    public class SectorController : ControllerBase
    {
        private readonly SectorRepo _repo;

        public SectorController(SectorRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult> Post(string code, [FromBody] SectorEntity sector)
        {
            sector.SectorCode = code; // ensure consistency
            var success = await _repo.InsertOne(sector);
            return success ? Ok() : BadRequest();
        }

        [HttpPut]
        [Route("{code}")]
        public async Task<ActionResult> Put(string code, [FromBody] SectorEntity sector)
        {
            sector.SectorCode = code; // ensure consistency
            var success = await _repo.ReplaceOne(sector);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete]
        [Route("{code}")]
        public async Task<ActionResult> Delete(string code)
        {
            var success = await _repo.DeleteOne(code);
            return success ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int count = 10)
        {
            var sectorList = await _repo.Enumerate(page, count);
            return Ok(sectorList);
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var sector = await _repo.FindOneByCode(code);
            return sector != null ? Ok(sector) : NotFound();
        }
    }
}