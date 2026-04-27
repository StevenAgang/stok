using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using stok.Repository.Configurations.Helper;
using stok.Repository.Interace.Scraping;
using stok.Repository.Service.Hubs;
using stok.Repository.ViewModel.Scraping;

namespace stok.Controllers
{
    [ApiController]
    [Route("stok")]
    public class StokController(IScrapingService scrape, ResponseHelper response, IHubContext<ScrapingHubService> hubContext) : ControllerBase
    {
        private readonly IScrapingService _scrape = scrape;
        private readonly ResponseHelper _response = response;
        private readonly IHubContext<ScrapingHubService> _hubContext = hubContext;  
        [HttpGet("carinfo")]
        public async Task<IActionResult> Scrape(string url, string priceAnchor, string urlAnchor, int expectedOutput, CancellationToken cancellation)
        {
            //var result = await _scrape.Crawl(url, priceAnchor, urlAnchor, expectedOutput, cancellation);

            //return StatusCode(200, _response.Status(200,true, "Successfully fetched", result));

            throw new NotImplementedException();
        }

        [HttpPost("send-job")]
        public async Task<IActionResult> ScrapeJob([FromBody]ScrapingHubViewModel data)
        {
            await _hubContext.Clients.All.SendAsync("ReceivedScrapeJob", data);

            return StatusCode(200, _response.Status(200, true, "Successfully send job", null));
        }
    }
}
