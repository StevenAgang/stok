using stok.Repository.Interace.Data.Scraping;
using stok.Repository.Interace.Scraping;
using stok.Repository.Interace.TokenManager;
using stok.Repository.Model.TokenManager;
using System;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace stok.Repository.Service.Scraping
{
    public class ScrapingService(ITokenManagerService _tokenManager, IScrapingData _scrapingData) : IScrapingService
    {
       public async Task<string> GenerateKey(int userId)
       {
            string key = _tokenManager.GenerateUrlToken();

            var data = new ScrapeServiceTokenManager
            {
                UserAccountId = userId,
                Token = key,
                Created_At = DateTime.UtcNow,
                Created_By = userId,
                IsActive = true
            };

            await _scrapingData.Save(data);

            return key;
       }
    }
}
