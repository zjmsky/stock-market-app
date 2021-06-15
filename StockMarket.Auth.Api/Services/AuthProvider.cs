using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using StockMarket.Auth.Api.Models;
using StockMarket.Auth.Api.Entities;

namespace StockMarket.Auth.Api.Services
{
    public class AuthProvider
    {
        private readonly DatabaseContext _context;
        private readonly AuthConfig _config;

        public AuthProvider(DatabaseContext context, IOptions<AuthConfig> config)
        {
            _context = context;
            _config = config.Value;
        }

        public async Task<bool> Register(string username, string password, string email)
        {
            // construct general user object
            var user = new UserEntity
            {
                Username = username,
                Password = password,
                Role = UserRole.General,
                Email = email,
                IsVerified = false,
                RefreshTokens = new List<RefreshTokenEntity>(),
            };

            if (user.Validate().Count > 0)
                return false;

            // try to insert user into database
            try { await _context.Users.InsertOneAsync(user); }
            catch (MongoWriteException) { return false; }

            return true;
        }

        public async Task< AuthResponse> Authenticate(string username, string password, string ipAddress)
        {
            // retrieve user document
            var dbUser = await _context.Users
                .Find(u => u.Username == username)
                .FirstOrDefaultAsync();
            
            if (dbUser == null)
                return AuthResponse.Failure("invalid username");
            if (dbUser.Password != password)
                return AuthResponse.Failure("invalid password");
            if (dbUser.IsVerified == false)
                return AuthResponse.Failure("unverified user");

            // generate and push refresh token to user document
            var refreshToken = GenerateRefreshToken(dbUser);
            var dbToken = new RefreshTokenEntity(refreshToken, ipAddress);
            var tokenUpdate = Builders<UserEntity>.Update.Push("RefreshTokens", dbToken);
            await _context.Users.FindOneAndUpdateAsync(u => u.Username == username, tokenUpdate);

            // generate access token
            var accessToken = GenerateAccessToken(dbUser);

            return AuthResponse.Success(accessToken, refreshToken);
        }

        public async Task<AuthResponse> Refresh(string refreshToken, string ipAddress)
        {
            // retrieve user document
            var userId = GetUserIdFromToken(refreshToken);

            var dbUser = await _context.Users
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (dbUser == null)
                return AuthResponse.Failure("invalid user - id:" + userId);

            // remove expired tokens from user document
            dbUser.RefreshTokens.RemoveAll(t => t.HasExpired());
            var tokenUpdate = Builders<UserEntity>.Update.Set("RefreshTokens", dbUser.RefreshTokens);
            await _context.Users.FindOneAndUpdateAsync(u => u.Id == userId, tokenUpdate);

            // match the token string and ip address
            var dbToken = dbUser.RefreshTokens.Find(t => t.Token == refreshToken);
            if (dbToken == null || dbToken.IpAddress != ipAddress)
                return AuthResponse.Failure("invalid token");

            // generate access token
            var accessToken = GenerateAccessToken(dbUser);

            return AuthResponse.Success(accessToken, dbToken.Token);
        }

        public async Task<bool> Revoke(string refreshToken)
        {
            // retrieve user document
            var userId = GetUserIdFromToken(refreshToken);
            var dbUser = await _context.Users
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();
            if (dbUser == null)
                return false;

            // retrieve matching token object
            var dbToken = dbUser.RefreshTokens.Find(t => t.Token == refreshToken);
            if (dbToken == null)
                return false;

            // remove matching token object from user document
            var tokenUpdate = Builders<UserEntity>.Update.Pull("RefreshTokens", dbToken);
            await _context.Users.FindOneAndUpdateAsync(u => u.Id == userId, tokenUpdate);

            return true;
        }

        private string GenerateAccessToken(UserEntity user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
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

        private string GenerateRefreshToken(UserEntity user)
        {
            // encode user id => 12 bytes = 16 characters in base64
            var idBytes = user.Id.ToByteArray();
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
