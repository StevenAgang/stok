using stok.Repository.ViewModel.Scraping;

namespace stok.Repository.Interace.Scraping
{
    public interface IScrapingService
    {
      Task<string> GenerateKey(int userId);
    }
}
