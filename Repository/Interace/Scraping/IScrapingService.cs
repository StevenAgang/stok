using AngleSharp.Dom;
using stok.Repository.ViewModel.Scraping;

namespace stok.Repository.Interace.Scraping
{
    public interface IScrapingService
    {
        Task<IEnumerable<ScrapingViewModel>> Crawl(string url, CancellationToken cancellation);
    }
}
