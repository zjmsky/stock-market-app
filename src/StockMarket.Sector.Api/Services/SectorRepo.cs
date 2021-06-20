using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StockMarket.Sector.Api.Models;
using StockMarket.Sector.Api.Entities;

namespace StockMarket.Sector.Api.Services
{
    public class SectorRepo
    {
        private readonly DatabaseContext _context;
        private readonly EventBus _events;

        public SectorRepo(DatabaseContext context, EventBus events)
        {
            _context = context;
            _events = events;
        }

        private async Task<bool> ValidateReferences(SectorEntity sector)
        {
            // sector does not depend on any entity
            // so nothing for now
            await Task.Yield();
            return true;
        }

        private async Task<bool> AssertNoReferences(SectorEntity sector)
        {
            // assert no reference in company collection
            var companyExists = await _context.Companies
                .Find(c => c.SectorCode == sector.SectorCode)
                .AnyAsync();
            if (companyExists) return false;

            return true;
        }

        private async Task<bool> InsertOneUnchecked(SectorEntity sector)
        {
            try { await _context.Sectors.InsertOneAsync(sector); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> ReplaceOneUnchecked(SectorEntity sector)
        {
            var code = sector.SectorCode;
            try { await _context.Sectors.ReplaceOneAsync(e => e.SectorCode == code, sector); }
            catch (MongoWriteException) { return false; }
            return true;
        }

        private async Task<bool> DeleteOneUnchecked(SectorEntity sector)
        {
            var code = sector.SectorCode;
            var result = await _context.Sectors.DeleteOneAsync(e => e.SectorCode == code);
            return result.DeletedCount > 0;
        }

        private async Task<bool> PublishUpdation(SectorEntity sector)
        {
            var message = new SectorIntegrationEvent(EventType.Update, sector);
            await _events.PublishSectorEvent(message);
            return true;
        }

        private async Task<bool> PublishDeletion(SectorEntity sector)
        {
            var message = new SectorIntegrationEvent(EventType.Delete, sector);
            await _events.PublishSectorEvent(message);
            return true;
        }

        public async Task<bool> InsertOne(SectorEntity sector)
        {
            return sector.Sanitize().Validate().Count == 0
                && await ValidateReferences(sector)
                && await InsertOneUnchecked(sector)
                && await PublishUpdation(sector);
        }

        public async Task<bool> ReplaceOne(SectorEntity sector)
        {
            return sector.Sanitize().Validate().Count == 0
                && await ValidateReferences(sector)
                && await ReplaceOneUnchecked(sector)
                && await PublishUpdation(sector);
        }

        public async Task<bool> DeleteOne(string code)
        {
            var sector = await FindOneByCode(code);
            return sector != null
                && await AssertNoReferences(sector)
                && await DeleteOneUnchecked(sector)
                && await PublishDeletion(sector);
        }

        public async Task<List<SectorEntity>> Enumerate(int page = 1, int count = 10)
        {
            // ensure valid values
            page = Math.Max(page, 1);
            count = Math.Clamp(count, 1, 50);
            return await _context.Sectors
                .Find(x => true)
                .Skip((page - 1) * count)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<SectorEntity> FindOneByCode(string code)
        {
            // case insensitive
            code = code.ToUpper();
            return await _context.Sectors
                .Find(s => s.SectorCode == code)
                .FirstOrDefaultAsync();
        }
    }
}