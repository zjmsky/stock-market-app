using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Sector.Api.Services;

namespace StockMarket.Sector.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectorController : ControllerBase
    {
        private readonly SectorRepo _repo;

        public SectorController(SectorRepo repo)
        {
            _repo = repo;
        }

        [HttpPut]
        [Route("{code}")]
        public async Task<ActionResult> Put(string code, [FromBody] Entities.Sector sector)
        {
            sector.SectorCode = code.ToUpper(); // just to be sure
            var success = await _repo.InsertOrReplaceOne(sector);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpDelete]
        [Route("{code}")]
        public async Task<ActionResult> Delete(string code)
        {
            var success = await _repo.DeleteOne(code.ToUpper());
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int count = 10)
        {
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 25);
            var sectorList = await _repo.ListPage(page, count);
            var payload = new { page, count = sectorList.Count, sectors = sectorList };
            return Ok(payload);
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var sector = await _repo.FindOneByCode(code.ToUpper());
            return sector != null ? Ok(sector) : NotFound(null);
        }
    }
}