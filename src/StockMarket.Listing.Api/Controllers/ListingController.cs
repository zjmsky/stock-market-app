using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Listing.Api.Entities;
using StockMarket.Listing.Api.Services;

namespace StockMarket.Listing.Api.Controllers
{
    [ApiController]
    [Route("listings")]
    public class ListingController : ControllerBase
    {
        private readonly ListingRepo _repo;

        public ListingController(ListingRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("{exchange}:{ticker}")]
        public async Task<ActionResult> Post(string exchange, string ticker, [FromBody] ListingEntity listing)
        {
            // ensure consistency
            listing.ExchangeCode = exchange;
            listing.TickerSymbol = ticker;
            var success = await _repo.InsertOne(listing);
            return success ? Ok() : BadRequest();
        }

        [HttpPut("{exchange}:{ticker}")]
        public async Task<ActionResult> Put(string exchange, string ticker, [FromBody] ListingEntity listing)
        {
            // ensure consistency
            listing.ExchangeCode = exchange;
            listing.TickerSymbol = ticker;
            var success = await _repo.ReplaceOne(listing);
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
            var listingList = await _repo.Enumerate(page, count);
            return Ok(listingList);
        }

        [HttpGet("{company}")]
        public async Task<ActionResult> Get(string company)
        {
            var listingList = await _repo.FindByCompany(company);
            return Ok(listingList);
        }

        [HttpGet("{exchange}:{ticker}")]
        public async Task<ActionResult> Get(string exchange, string ticker)
        {
            var listing = await _repo.FindOneByTicker(exchange, ticker);
            return listing != null ? Ok(listing) : NotFound();
        }
    }
}