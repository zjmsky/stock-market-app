using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StockMarket.Auth.Api.Services;
using StockMarket.Auth.Api.Models;

namespace StockMarket.Auth.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthProvider _authProvider;

        public AuthController(AuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var email = request.Email;
            var success = await _authProvider.Register(username, password, email);
            return success ? Ok() : BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var username = request.Username;
            var password = request.Password;
            var deviceId = request.DeviceId;
            var result = await _authProvider.Authenticate(username, password, deviceId);
            return result.IsSuccess() ? Ok(result.GetValue()) : BadRequest(result.GetError());
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var refreshToken = request.RefreshToken;
            var deviceId = request.DeviceId;
            var result = await _authProvider.Refresh(refreshToken, deviceId);
            return result.IsSuccess() ? Ok(result.GetValue()) : BadRequest(result.GetError());
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            var refreshToken = request.RefreshToken;
            var deviceId = request.DeviceId;
            var success = await _authProvider.Revoke(refreshToken, deviceId);
            return success ? Ok() : BadRequest();
        }
    }
}
