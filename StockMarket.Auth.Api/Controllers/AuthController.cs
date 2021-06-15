using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StockMarket.Auth.Api.Services;
using StockMarket.Auth.Api.Models;

namespace StockMarket.Auth.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthProvider _authProvider;

        public AuthController(AuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var email = request.Email;
            var success = await _authProvider.Register(username, password, email);
            return success ? Ok() : BadRequest();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var ipAddress = GetIpAddress();
            var response = await _authProvider.Authenticate(username, password, ipAddress);
            if (response.IsSuccess())
            {
                var accessToken = response.AccessToken;
                var refreshToken = response.RefreshToken;
                var payload = new { accessToken, refreshToken };
                return Ok(payload);
            }
            else
            {
                var error = response.Error;
                var payload = new { error };
                return BadRequest(payload);
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh([FromHeader] string refreshToken)
        {
            var ipAddress = GetIpAddress();
            var response = await _authProvider.Refresh(refreshToken, ipAddress);
            return response.IsSuccess() ? Ok(response) : BadRequest(response);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromHeader] string refreshToken)
        {
            var success = await _authProvider.Revoke(refreshToken);
            return success ? Ok() : BadRequest();
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"][0];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
