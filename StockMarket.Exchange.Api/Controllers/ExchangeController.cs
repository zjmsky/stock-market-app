using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPut]
        [Route("{code}")]
        public async Task<ActionResult> Put(string code, [FromBody] Entities.Exchange exchange)
        {
            exchange.ExchangeCode = code.ToUpper(); // just to be sure
            var success = await _repo.InsertOrReplaceOne(exchange);
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
            var exchangeList = await _repo.ListPage(page, count);
            var payload = new { page, count = exchangeList.Count, exchanges = exchangeList };
            return Ok(payload);
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var exchange = await _repo.FindOneByCode(code.ToUpper());
            return exchange != null ? Ok(exchange) : NotFound(null);
        }
    }
}
