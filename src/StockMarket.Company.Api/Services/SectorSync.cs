using System.Threading.Tasks;
using StockMarket.Company.Api.Models;

namespace StockMarket.Company.Api.Services
{
    public class SectorSync
    {
        private readonly SectorRepo _repo;

        public SectorSync(SectorRepo repo, EventBus events)
        {
            _repo = repo;
            events.SubscribeSectorEvent((ev) => OnIntegrationEvent(ev));
        }

        private async Task OnIntegrationEvent(SectorIntegrationEvent integrationEvent)
        {
            if (integrationEvent.IsUpdation())
                await OnUpdationEvent(integrationEvent);
            else if (integrationEvent.IsDeletion())
                await OnDeletionEvent(integrationEvent);
        }

        private async Task OnUpdationEvent(SectorIntegrationEvent updationEvent)
        {
            var sector = updationEvent.ToEntity();
            await _repo.InsertOrReplaceOneUnchecked(sector);
        }

        private async Task OnDeletionEvent(SectorIntegrationEvent deletionEvent)
        {
            var sector = await _repo.FindOneByCode(deletionEvent.SectorCode);
            await _repo.DeleteOneUnchecked(sector);
        }
    }
}