using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Listing.Api.Entities;
using StockMarket.Listing.Api.Services;

namespace StockMarket.Listing.Api.Controllers
{
    [ApiController]
    [Route("ipos")]
    public class IpoController : ControllerBase
    {
        private readonly IpoRepo _repo;

        public IpoController(IpoRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("{exchange}:{ticker}")]
        public async Task<ActionResult> Post(string exchange, string ticker, [FromBody] IpoEntity ipo)
        {
            // ensure consistency
            ipo.ExchangeCode = exchange;
            ipo.TickerSymbol = ticker;
            var success = await _repo.InsertOne(ipo);
            return success ? Ok() : BadRequest();
        }

        [HttpPut("{exchange}:{ticker}")]
        public async Task<ActionResult> Put(string exchange, string ticker, [FromBody] IpoEntity ipo)
        {
            // ensure consistency
            ipo.ExchangeCode = exchange;
            ipo.TickerSymbol = ticker;
            var success = await _repo.ReplaceOne(ipo);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{exchange}:{ticker}")]
        public async Task<ActionResult> Delete(string exchange, string ticker)
        {
            var success = await _repo.DeleteOne(exchange, ticker);
            return success ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int count = 10)
        {
            var ipoList = await _repo.Enumerate(page, count);
            return Ok(ipoList);
        }

        [HttpGet("{exchange}:{ticker}")]
        public async Task<ActionResult> Get(string exchange, string ticker)
        {
            var ipo = await _repo.FindOneByTicker(exchange, ticker);
            return ipo != null ? Ok(ipo) : NotFound();
        }
    }
}