using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Exchange.Api.Services;
using StockMarket.Exchange.Api.Entities;

namespace StockMarket.Exchange.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly ExchangeRepo _repo;

        public ExchangeController(ExchangeRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult> Post(string code, [FromBody] ExchangeEntity exchange)
        {
            exchange.ExchangeCode = code; // ensure consistency
            var success = await _repo.InsertOne(exchange);
            return success ? Ok() : BadRequest();
        }

        [HttpPut]
        [Route("{code}")]
        public async Task<ActionResult> Put(string code, [FromBody] ExchangeEntity exchange)
        {
            exchange.ExchangeCode = code; // ensure consistency
            var success = await _repo.ReplaceOne(exchange);
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
            var exchangeList = await _repo.Enumerate(page, count);
            return Ok(exchangeList);
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var exchange = await _repo.FindOneByCode(code);
            return exchange != null ? Ok(exchange) : NotFound();
        }
    }
}
