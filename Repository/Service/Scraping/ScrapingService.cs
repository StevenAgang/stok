using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Playwright;
using stok.Repository.Interace.Scraping;
using stok.Repository.ViewModel.Scraping;
using System;
using System.Net;
using System.Text.Json;

namespace stok.Repository.Service.Scraping
{
    public class ScrapingService(IHttpClientFactory factory) : IScrapingService
    {
        private readonly HttpClient _client = factory.CreateClient();
        public async Task<IEnumerable<ScrapingViewModel>> Crawl(string url, CancellationToken cancellation)
        {

            var html = await MakeRequest(url, cancellation);

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            var anchors = document.QuerySelectorAll("a[href]");
            var jsonScripts = document.QuerySelectorAll("script[type='application/ld+json']");

            var domain = url.Split("/");

            IEnumerable<string> anchorNavigate = [];
            IEnumerable<string> jsonNavigate = [];

            if (anchors.Length > 0)
            {
                anchorNavigate = await FilterAnchorLinks(anchors, domain[2], cancellation);
            }
            if (jsonScripts.Length > 0)
            {
               jsonNavigate = await FilterJsonLinks(jsonScripts, cancellation);
            }

            return await ExtractLinks(anchorNavigate.Concat(jsonNavigate), cancellation);
        }


        private async Task<string> MakeRequest(string url, CancellationToken cancellation)
        {
            var response = await SimapleRequest(url, cancellation);

            string html = "";

            if (response.StatusCode != HttpStatusCode.Forbidden)
            {
                html = await response.Content.ReadAsStringAsync();
            }
            else
            {
                html = await BrowserRequest(url);
            }

            return html;
        }
        private async Task<HttpResponseMessage> SimapleRequest(string url, CancellationToken cancellation)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Referrer = new Uri(url);

            return await _client.SendAsync(request, cancellation);
        }

        private async Task<string> BrowserRequest(string url)
        {
            var play = await Playwright.CreateAsync();
            var browser = await play.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
            var html = await page.ContentAsync();
            await browser.CloseAsync();

            return html;
        }

        private Task<IEnumerable<string>> FilterAnchorLinks(IHtmlCollection<IElement> anchors, string filter, CancellationToken cancellation)
        {
            List<string> navigate = [];

            foreach (var a in anchors)
            {
                cancellation.ThrowIfCancellationRequested();
                var href = a.GetAttribute("href");
                if (!string.IsNullOrWhiteSpace(href) && href.Contains($"https://{filter}"))
                {
                    navigate.Add(href);
                }
            }
            return Task.FromResult<IEnumerable<string>>(navigate);
        }

        private Task<IEnumerable<string>> FilterJsonLinks(IHtmlCollection<IElement> jsonScripts, CancellationToken cancellation)
        {
            List<string> jsonNavigate = [];

            foreach (var script in jsonScripts)
            {
                cancellation.ThrowIfCancellationRequested();
                var json = script.TextContent;
                var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("itemListElement", out var items))
                {
                    cancellation.ThrowIfCancellationRequested();
                    foreach (var item in items.EnumerateArray())
                    {
                        cancellation.ThrowIfCancellationRequested();
                        foreach (var prop in item.EnumerateObject())
                        {
                            cancellation.ThrowIfCancellationRequested();
                            if (prop.Name.Equals("url", StringComparison.OrdinalIgnoreCase) || prop.Name.Equals("link", StringComparison.OrdinalIgnoreCase) || prop.Name.Equals("Url", StringComparison.OrdinalIgnoreCase))
                            {
                                jsonNavigate.Add(prop.Value.GetString());
                            }
                        }
                    }
                }
            }
            return Task.FromResult<IEnumerable<string>>(jsonNavigate);
        }

        private async Task<IEnumerable<ScrapingViewModel>> ExtractLinks(IEnumerable<string> links, CancellationToken cancellation)
        {
            List<ScrapingViewModel> scrapingResults = [];

            foreach (var link in links)
            {
                cancellation.ThrowIfCancellationRequested();

                try
                {

                    var html = await MakeRequest(link, cancellation);
                    var context = BrowsingContext.New(Configuration.Default);
                    var document = await context.OpenAsync(req => req.Content(html));

                    var ldJson = document.QuerySelectorAll("script[type='application/ld+json']");

                    if (ldJson.Length > 0)
                    {
                        foreach (var ld in ldJson)
                        {
                            var jsonText = ld.TextContent;
                            var doc = JsonDocument.Parse(jsonText);

                            doc.RootElement.TryGetProperty("sku", out var skuElement);
                            doc.RootElement.TryGetProperty("price", out var priceElement);
                            doc.RootElement.TryGetProperty("priceCurrency", out var currencyElement);

                            var sku = skuElement.ValueKind == JsonValueKind.String ? skuElement.GetString() : null;
                            var price = priceElement.ValueKind == JsonValueKind.String ? priceElement.GetString() : null;
                            var currency = currencyElement.ValueKind == JsonValueKind.String ? currencyElement.GetString() : null; 
                            if (sku != null)
                            {
                                var stockNumber = sku;
                                //var carPrice = float.Parse(price);

                                scrapingResults.Add(new ScrapingViewModel
                                {
                                    Stock = stockNumber,
                                    Price = 0
                                });
                                break;
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }


            return scrapingResults;
        }
    }
}
