using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Company.Api.Services;

namespace StockMarket.Company.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly CompanyRepo _repo;

        public ExchangeController(CompanyRepo repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page, [FromQuery] int count)
        {
            var companyList = await _repo.ListPage(page, count);
            var payload = new { page = page, count = companyList.Count, companies = companyList };
            return Ok(payload);
        }

        [HttpGet]
        [Route("{exchange}:{ticker}")]
        public async Task<ActionResult> Get(string exchange, string ticker)
        {
            var company = await _repo.FindOneByTicker(ticker, exchange);
            return company != null ? Ok(company) : NotFound(null);
        }

        [HttpGet]
        [Route("{ticker}")]
        public async Task<ActionResult> Get(string ticker)
        {
            var company = await _repo.FindOneByTicker(ticker);
            return company != null ? Ok(company) : NotFound(null);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Entities.Company company)
        {
            var success = await _repo.InsertOne(company);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Entities.Company company)
        {
            var success = await _repo.UpdateOne(company);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }
    }
}