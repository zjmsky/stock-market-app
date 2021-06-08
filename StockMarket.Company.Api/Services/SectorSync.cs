using System.Threading.Tasks;
using MongoDB.Driver;
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
            var creationEvent = integrationEvent as SectorCreationEvent;
            var deletionEvent = integrationEvent as SectorDeletionEvent;

            if (creationEvent != null)
                await OnCreationEvent(creationEvent);
            else if (deletionEvent != null)
                await OnDeletionEvent(deletionEvent);
        }

        private async Task OnCreationEvent(SectorCreationEvent creationEvent)
        {
            var sector = creationEvent.ToEntity();
            var exists = await _context.Sectors.Find(s => s.Id == sector.Id).AnyAsync();

            if (!exists)
                await _context.Sectors.InsertOneAsync(sector);
            else
                await _context.Sectors.ReplaceOneAsync(s => s.Id == sector.Id, sector);
        }

        private async Task OnDeletionEvent(SectorDeletionEvent deletionEvent)
        {
            var sectorId = deletionEvent.ToId();
            await _context.Sectors.DeleteOneAsync(s => s.Id == sectorId);
        }
    }
}