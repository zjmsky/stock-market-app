using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StockMarket.Auth.Api.Services;
using StockMarket.Auth.Api.Models;

namespace StockMarket.Auth.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthProvider _authProvider;

        public AuthController(AuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] SignupRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var email = request.Email;
            var success = await _authProvider.Signup(username, password, email);
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var ipAddress = GetIpAddress();
            var response = await _authProvider.Authenticate(username, password, ipAddress);
            return response.IsSuccess() ? Ok(response) : BadRequest(response);
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
            var payload = new { success };
            return success ? Ok(payload) : BadRequest(payload);
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
