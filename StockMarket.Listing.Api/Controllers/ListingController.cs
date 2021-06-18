using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Listing.Api.Entities;
using StockMarket.Listing.Api.Services;

namespace StockMarket.Listing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListingController : ControllerBase
    {
        private readonly ListingRepo _repo;

        public ListingController(ListingRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Post(string exchange, string ticker, [FromBody] ListingEntity listing)
        {
            // ensure consistency
            listing.ExchangeCode = exchange;
            listing.TickerSymbol = ticker;
            var success = await _repo.InsertOne(listing);
            return success ? Ok() : BadRequest();
        }

        [HttpPut]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Put(string exchange, string ticker, [FromBody] ListingEntity listing)
        {
            // ensure consistency
            listing.ExchangeCode = exchange;
            listing.TickerSymbol = ticker;
            var success = await _repo.ReplaceOne(listing);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Delete(string exchange, string ticker)
        {
            var success = await _repo.DeleteOne(exchange, ticker);
            return success ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int count = 10)
        {
            var sectorList = await _repo.Enumerate(page, count);
            return Ok(sectorList);
        }

        [HttpGet]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Get(string exchange, string ticker)
        {
            var sector = await _repo.FindOneByTicker(exchange, ticker);
            return sector != null ? Ok(sector) : NotFound();
        }
    }
}