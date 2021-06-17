using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Company.Api.Entities;
using StockMarket.Company.Api.Services;

namespace StockMarket.Company.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyRepo _repo;

        public CompanyController(CompanyRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult> Post(string code, [FromBody] CompanyEntity company)
        {
            company.CompanyCode = code; // ensure consistency
            var success = await _repo.InsertOne(company);
            return success ? Ok() : BadRequest();
        }

        [HttpPut]
        [Route("{code}")]
        public async Task<ActionResult> Put(string code, [FromBody] CompanyEntity company)
        {
            company.CompanyCode = code; // ensure consistency
            var success = await _repo.ReplaceOne(company);
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
            var companyList = await _repo.Enumerate(page, count);
            return Ok(companyList);
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            var company = await _repo.FindOneByCode(code);
            return company != null ? Ok(company) : NotFound();
        }
    }
}