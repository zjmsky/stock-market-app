using System.Threading.Tasks;
using StockMarket.Listing.Api.Models;

namespace StockMarket.Listing.Api.Services
{
    public class CompanySync
    {
        private readonly CompanyRepo _repo;

        public CompanySync(CompanyRepo repo, EventBus events)
        {
            _repo = repo;
            events.Subscribe<CompanyIntegrationEvent>("ListingApi", OnIntegrationEvent);
        }

        private async Task OnIntegrationEvent(CompanyIntegrationEvent integrationEvent)
        {
            if (integrationEvent.IsUpdationEvent())
                await OnUpdationEvent(integrationEvent);
            else if (integrationEvent.IsDeletionEvent())
                await OnDeletionEvent(integrationEvent);
        }

        private async Task OnUpdationEvent(CompanyIntegrationEvent updationEvent)
        {
            var company = updationEvent.ToEntity();
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