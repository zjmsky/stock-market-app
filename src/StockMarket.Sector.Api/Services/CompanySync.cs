using System.Threading.Tasks;
using StockMarket.Sector.Api.Models;

namespace StockMarket.Sector.Api.Services
{
    public class CompanySync
    {
        private readonly CompanyRepo _repo;

        public CompanySync(CompanyRepo repo, EventBus events)
        {
            _repo = repo;
            events.SubscribeCompanyEvent((ev) => OnIntegrationEvent(ev));
        }

        private async Task OnIntegrationEvent(CompanyIntegrationEvent integrationEvent)
        {
            if (integrationEvent.IsUpdation())
                await OnUpdationEvent(integrationEvent);
            else if (integrationEvent.IsDeletion())
                await OnDeletionEvent(integrationEvent);
        }

        private async Task OnUpdationEvent(CompanyIntegrationEvent creationEvent)
        {
            var company = creationEvent.ToEntity();
            await _repo.InsertOrReplaceOneUnchecked(company);
        }

        private async Task OnDeletionEvent(CompanyIntegrationEvent deletionEvent)
        {
            var code = deletionEvent.CompanyCode;
            var company = await _repo.FindOneByCode(code);
            await _repo.DeleteOneUnchecked(company);
        }
    }
}