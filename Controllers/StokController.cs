using Microsoft.AspNetCore.Mvc;
using stok.Repository.Configurations.Helper;
using stok.Repository.Interace.Scraping;
using stok.Repository.ViewModel.Scraping;

namespace stok.Controllers
{
    [ApiController]
    [Route("stok")]
    public class StokController(IScrapingService scrape, ResponseHelper response) : ControllerBase
    {
        private readonly IScrapingService _scrape = scrape;
        private readonly ResponseHelper _response = response;


        [HttpGet("carinfo")]
        public async Task<IActionResult> Scrape(string url, CancellationToken cancellation)
        {
            var result = await _scrape.Crawl(url, cancellation);

            return StatusCode(200, _response.Status(200,true, "Successfully fetched", result));
        }
    }
}
