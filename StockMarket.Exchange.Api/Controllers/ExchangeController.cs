using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Models.Exchange exchange)
        {
            try
            {
                await _repo.InsertOneAsync(exchange);
                return Ok(new { ok = true, error = "" });
            }
            catch (ValidationException ex)
            {
                var fieldName = Regex.Replace(ex.Message, "([A-Z])", " $1");
                var error = $"invalid {fieldName.Trim().ToLower()}";
                return BadRequest(new { ok = false, error = error });
            }
            catch (MongoWriteException)
            {
                return BadRequest(new { ok = false, error = "duplicate record" });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Models.Exchange>>> Get()
        {
            return Ok(await _repo.ListPageAsync());
        }

        [HttpGet]
        [Route(":{code}")]
        public async Task<ActionResult> Get(string code)
        {
            return Ok(await _repo.FindOneAsync(x => x.CountryCode == code));
        }
    }
}
