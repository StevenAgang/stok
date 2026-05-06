using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using stok.Repository.Interace.Scraping.Hubs;
using stok.Repository.ViewModel.Scraping;

namespace stok.Repository.Service.Hubs
{
    public class ScrapingHubService(IMemoryCache _cache) : Hub<IScrapingHubService>
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var key = httpContext.Request.Query["key"].ToString();

            if (!string.IsNullOrEmpty(key))
            {
                _cache.TryGetValue(Context.ConnectionId, out var result);

                if(result == null)
                {
                    _cache.Set(Context.ConnectionId,key);
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, key.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _cache.TryGetValue(Context.ConnectionId, out var key);

           if(key != null)
            {
                _cache.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, key.ToString());
            }
            await base.OnDisconnectedAsync(exception);
        }

        public Task<bool> CheckConnection(string key)
        {
            _cache.TryGetValue(Context.ConnectionId, out var keys);

            if(keys == null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        } 

        public async Task SendScrapeJob(string key, ScrapeLink url)
        {
            await Clients.Group(key).ReceivedScrapeJob(url);
        }

        public async Task BroadCastScrapingLinkResult(string key, IEnumerable<ScrapingLinkResult> result)
        {
            await Clients.Group(key).ReceivedScrapingLinkResult(result);
        }

        public async Task ScrapeLink(string key, IEnumerable<ScrapingLinkResult> links)
        {
            await Clients.Group(key).ReceivedLinkToScrape(links);
        }

        public async Task BroadcastScrapingResult(string key, IEnumerable<ScrapingViewModel> result)
        {
            await Clients.Group(key).ReceivedScrapingResult(result);
        }
    }
}
