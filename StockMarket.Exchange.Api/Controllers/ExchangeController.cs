using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StockMarket.Exchange.Api.Services;

namespace StockMarket.Exchange.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly ExchangeRepo _repo;

        public ExchangeController(ExchangeRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Entities.Exchange exchange)
        {
            var success = await _repo.InsertOne(exchange);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put([FromBody] Entities.Exchange exchange)
        {
            var success = await _repo.ReplaceOne(exchange);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var success = await _repo.DeleteOne(id);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page, [FromQuery] int count)
        {
            var exchangeList = await _repo.ListPage(page, count);
            var payload = new { page, count = exchangeList.Count, exchanges = exchangeList };
            return Ok(payload);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var exchange = await _repo.FindOneById(id);
            return exchange != null ? Ok(exchange) : NotFound(null);
        }
    }
}
