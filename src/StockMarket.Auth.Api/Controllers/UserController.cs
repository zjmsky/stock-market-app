using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StockMarket.Auth.Api.Services;
using StockMarket.Auth.Api.Models;

namespace StockMarket.Auth.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly UserRepo _repo;

        public UserController(UserRepo repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = UserRole.General)]
        [HttpGet("{username}")]
        public async Task<ActionResult> Get(string username)
        {
            var user = await _repo.FindOneByUsername(username);
            return user != null ? Ok(user) : NotFound();
        }
    }
}
