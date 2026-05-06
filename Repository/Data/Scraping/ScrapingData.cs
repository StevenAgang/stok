using stok.Repository.Interace.Data.Scraping;

namespace stok.Repository.Data.Scraping
{
    public class ScrapingData(DatabaseContext context) : BaseData(context), IScrapingData
    {
    }
}
