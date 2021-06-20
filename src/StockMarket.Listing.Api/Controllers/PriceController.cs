using System;
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

        [HttpPost("{exchange}:{ticker}")]
        public async Task<ActionResult> Post(string exchange, string ticker, [FromBody] PriceEntity price)
        {
            // ensure consistency
            price.ExchangeCode = exchange;
            price.TickerSymbol = ticker;
            var success = await _repo.InsertOne(price);
            return success ? Ok() : BadRequest();
        }

        [HttpPut("{exchange}:{ticker}")]
        public async Task<ActionResult> Put(string exchange, string ticker, [FromBody] PriceEntity price)
        {
            // ensure consistency
            price.ExchangeCode = exchange;
            price.TickerSymbol = ticker;
            var success = await _repo.ReplaceOne(price);
            return success ? Ok() : BadRequest();
        }

        [HttpGet("{exchange}:{ticker}")]
        public async Task<ActionResult> Get(string exchange, string ticker, [FromQuery] DateTime fromTime, [FromQuery] DateTime toTime)
        {
            var ipoList = await _repo.Enumerate(exchange, ticker, fromTime, toTime);
            return Ok(ipoList);
        }
    }
}