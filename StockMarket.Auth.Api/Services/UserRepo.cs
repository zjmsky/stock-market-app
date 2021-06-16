using System;
using System.Threading.Tasks;
using StockMarket.Auth.Api.Entities;
using MongoDB.Driver;
using MongoDB.Bson;

namespace StockMarket.Auth.Api.Services
{
    public class UserRepo
    {
        private readonly DatabaseContext _context;

        public UserRepo(DatabaseContext context)
        {
            _context = context;
        }

        private async Task<bool> InsertOneUnchecked(UserEntity user)
        {
            try { await _context.Users.InsertOneAsync(user); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(UserEntity user)
        {
            try { await _context.Users.InsertOneAsync(user); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        public async Task<bool> InsertOne(UserEntity user)
        {
            return user.Validate().Count == 0
                && await InsertOneUnchecked(user);
        }

        public async Task<bool> ReplaceOne(UserEntity user)
        {
            return user.Validate().Count == 0
                && await ReplaceOneUnchecked(user);
        }


        public async Task<UserEntity> FindOneByUsername(string username)
        {
            return await _context.Users
                .Find(u => u.Username == username)
                .FirstOrDefaultAsync();
        }

        public async Task<UserEntity> FindOnyById(ObjectId id)
        {
            return await _context.Users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}