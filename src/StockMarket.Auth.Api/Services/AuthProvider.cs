using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using StockMarket.Auth.Api.Models;
using StockMarket.Auth.Api.Entities;

namespace StockMarket.Auth.Api.Services
{
    public class AuthProvider
    {
        private readonly AuthConfig _config;
        private readonly UserRepo _repo;

        public AuthProvider(IOptions<AuthConfig> config, UserRepo repo)
        {
            _config = config.Value;
            _repo = repo;
        }

        public async Task<bool> Register(string username, string password, string email)
        {
            var user = UserEntity.General(username, password, email);
            return await _repo.InsertOne(user);
        }

        public async Task<AuthResult> Authenticate(string username, string password, string deviceId)
        {
            var user = await _repo.FindOneByUsername(username);
            if (user == null || user.CheckPassword(password) == false)
                return AuthResult.Failure("invalid username or password");
            else if (user.IsVerified == false)
                return AuthResult.Failure("unverified user");

            var refreshToken = GenerateRefreshToken(user.Id);
            user.RefreshTokens.RemoveAll(r => r.DeviceId == deviceId);
            user.RefreshTokens.RemoveAll(r => r.HasExpired());
            user.RefreshTokens.Add(new RefreshTokenEntity(deviceId, refreshToken));
            if (await _repo.ReplaceOne(user) == false)
                return AuthResult.Failure("unknown error");

            var accessToken = GenerateAccessToken(user);

            return AuthResult.Success(accessToken, refreshToken);
        }

        public async Task<AuthResult> Refresh(string refreshToken, string deviceId)
        {
            var userId = GetUserIdFromToken(refreshToken);
            var user = await _repo.FindOnyById(userId);
            if (user == null)
                return AuthResult.Failure("invalid user");

            var storedToken = user.RefreshTokens.Find(r => r.DeviceId == deviceId);
            if (storedToken == null)
                return AuthResult.Failure("invalid device");
            else if (storedToken.HasExpired())
                return AuthResult.Failure("expired token");
            else if (storedToken.Token != refreshToken)
                return AuthResult.Failure("invalid token");

            var accessToken = GenerateAccessToken(user);

            return AuthResult.Success(accessToken, refreshToken);
        }

        public async Task<bool> Revoke(string refreshToken, string deviceId)
        {
            var userId = GetUserIdFromToken(refreshToken);
            var user = await _repo.FindOnyById(userId);
            return user != null
                && user.RefreshTokens.RemoveAll(r => r.DeviceId == deviceId) > 0
                && await _repo.ReplaceOne(user);
        }

        private string GenerateAccessToken(UserEntity user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var secretKey = Encoding.ASCII.GetBytes(_config.SecretKey);
            var securityKey = new SymmetricSecurityKey(secretKey);
            var securityAlg = SecurityAlgorithms.HmacSha256Signature;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(securityKey, securityAlg)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken(ObjectId userId)
        {
            // encode user id => 12 bytes = 16 characters in base64
            var idBytes = userId.ToByteArray();
            var idString = Convert.ToBase64String(idBytes).PadRight(16, '=');

            // encode random number => 48 bytes = 64 characters in base64
            var rngProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[48];
            rngProvider.GetBytes(randomBytes);
            var randomString = Convert.ToBase64String(randomBytes).PadRight(64, '=');

            // token = user id + random number => 60 bytes = 80 characters in base64
            return idString + randomString;
        }

        private ObjectId GetUserIdFromToken(string refreshToken)
        {
            var idString = refreshToken.Substring(0, 16);
            var idBytes = Convert.FromBase64String(idString);
            return new ObjectId(idBytes);
        }
    }
}
