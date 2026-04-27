using Microsoft.AspNetCore.SignalR;
using stok.Repository.Interace.Scraping.Hubs;
using stok.Repository.ViewModel.Scraping;

namespace stok.Repository.Service.Hubs
{
    public class ScrapingHubService : Hub<IScrapingHubService>
    {
        public async Task SendScrapeJob(ScrapeLink url)
        {
            await Clients.All.ReceivedScrapeJob(url);
        }

        public async Task BroadCastScrapingLinkResult(IEnumerable<ScrapingLinkResult> result)
        {
            await Clients.All.ReceivedScrapingLinkResult(result);
        }

        public async Task ScrapeLink(IEnumerable<ScrapingLinkResult> links)
        {
            await Clients.All.ReceivedLinkToScrape(links);
        }

        public async Task BroadcastScrapingResult(IEnumerable<ScrapingViewModel> result)
        {
            await Clients.All.ReceivedScrapingResult(result);
        }
    }
}
