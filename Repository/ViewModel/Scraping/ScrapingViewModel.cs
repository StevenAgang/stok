namespace stok.Repository.ViewModel.Scraping
{
    public class ScrapingViewModel
    {
        public string? Link { get; set; }
        public string? Stock { get; set; }
        public decimal Price { get; set; }
    }

    public class ScrapingLinkResult
    {
        public string? Results { get; set; }
    }

    public class ScrapeLink
    {
        public string? Url { get; set; }
        public string? UrlAnchor { get; set; }
    }

    public class ScrapingHubViewModel
    {
        public string? Url { get; set; }
        public string? PriceAnchor { get; set; }
        public string? SkipElement { get; set; }
    }
}
