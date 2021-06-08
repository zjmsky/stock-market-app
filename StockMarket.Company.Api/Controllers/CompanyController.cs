using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Company.Api.Entities;
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

        [HttpPut]
        [Route("{code}")]
        public async Task<ActionResult> Put(string code, [FromBody] CompanyEntity company)
        {
            company.CompanyCode = code; // ensure consistency
            var success = await _repo.InsertOrReplaceOne(company);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpDelete]
        [Route("{code}")]
        public async Task<ActionResult> Delete(string code)
        {
            var success = await _repo.DeleteOne(code);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int page, [FromQuery] int count)
        {
            var companyList = await _repo.Enumerate(page, count);
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
        [Route("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var company = await _repo.FindOneByCode(code);
            return company != null ? Ok(company) : NotFound(null);
        }
    }
}