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
    public class StokController(IScrapingService _scrape, ResponseHelper _response, IHubContext<ScrapingHubService> _hubContext) : ControllerBase
    {
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

        [HttpPost("generate-key")]
        public async Task<IActionResult> GenerateServiceKey([FromQuery] int userId)
        {
            string key = await _scrape.GenerateKey(userId);

            return StatusCode(200, _response.Status(200, true, "Key Created", key));
        }
    }
}
