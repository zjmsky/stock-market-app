using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using StockMarket.Company.Api.Models;

namespace StockMarket.Company.Api.Services
{
    public class SectorSync
    {
        private readonly DatabaseContext _context;

        public SectorSync(DatabaseContext context, EventBus events)
        {
            _context = context;
            events.Subscribe<ISectorIntegrationEvent>("CompanyApi", OnIntegrationEvent);
        }

        private async Task OnIntegrationEvent(ISectorIntegrationEvent integrationEvent)
        {
            var createdEvent = integrationEvent as SectorCreatedEvent;
            var deletedEvent = integrationEvent as SectorDeletedEvent;

            if (createdEvent != null)
                await OnCreatedEvent(createdEvent);
            else if (deletedEvent != null)
                await OnDeletedEvent(deletedEvent);
        }

        private async Task OnCreatedEvent(SectorCreatedEvent createdEvent)
        {
            var sector = new Entities.Sector
            {
                Id = new ObjectId(createdEvent.Id),
                SectorCode = createdEvent.SectorCode,
            };

            var exists = await _context.Sectors.Find(s => s.Id == sector.Id).AnyAsync();
            if (!exists)
                await _context.Sectors.InsertOneAsync(sector);
            else
                await _context.Sectors.ReplaceOneAsync(s => s.Id == sector.Id, sector);
        }

        private async Task OnDeletedEvent(SectorDeletedEvent deletedEvent)
        {
            var sectorId = new ObjectId(deletedEvent.Id);
            await _context.Sectors.DeleteOneAsync(s => s.Id == sectorId);
        }
    }
}