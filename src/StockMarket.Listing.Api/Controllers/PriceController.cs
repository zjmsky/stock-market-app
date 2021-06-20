using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Listing.Api.Entities;
using StockMarket.Listing.Api.Services;

namespace StockMarket.Listing.Api.Controllers
{
    [ApiController]
    [Route("prices")]
    public class PriceController : ControllerBase
    {
        private readonly PriceRepo _repo;

        public PriceController(PriceRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Post(string exchange, string ticker, [FromBody] PriceEntity price)
        {
            // ensure consistency
            price.ExchangeCode = exchange;
            price.TickerSymbol = ticker;
            var success = await _repo.InsertOne(price);
            return success ? Ok() : BadRequest();
        }

        [HttpPut]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Put(string exchange, string ticker, [FromBody] PriceEntity price)
        {
            // ensure consistency
            price.ExchangeCode = exchange;
            price.TickerSymbol = ticker;
            var success = await _repo.ReplaceOne(price);
            return success ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int count = 10)
        {
            var ipoList = await _repo.Enumerate(page, count);
            return Ok(ipoList);
        }
    }
}