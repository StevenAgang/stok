using stok.Repository.ViewModel.Scraping;

namespace stok.Repository.Interace.Scraping.Hubs
{
    public interface IScrapingHubService
    {
        Task ReceivedScrapeJob(ScrapeLink url);
        Task ReceivedScrapingLinkResult(IEnumerable<ScrapingLinkResult> result);
        Task ReceivedLinkToScrape(IEnumerable<ScrapingLinkResult> links);
        Task ReceivedScrapingResult(IEnumerable<ScrapingViewModel> result);
    }
}
