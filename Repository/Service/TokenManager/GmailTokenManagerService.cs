using Microsoft.Extensions.Caching.Memory;
using stok.Repository.Configurations;
using stok.Repository.Interace.TokenManager;
using System.Text.Json;
using System.Threading.Tasks;

namespace stok.Repository.Service.TokenManager
{
    public class GmailTokenManagerService(IMemoryCache _cache, IConfiguration _config) : IGmailTokenManagerService
    {

        private async Task RenewAccessToken()
        {
            var googleAuth = new GoogleAuthDefault();

            _config.GetSection("GoogleAuthDefault").Bind(googleAuth);

            var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", googleAuth.ClientId),
                new KeyValuePair<string, string>("client_secret", googleAuth.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", googleAuth.RefreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            });

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var token = doc.RootElement.GetProperty("access_token").GetString();

            SaveAccessToken(token);
        }

        private void SaveAccessToken(string token)
        {
            _cache.Set("GmailAccessToken", token, TimeSpan.FromHours(1));
        }

        public async Task<string?> GetAccessToken()
        {
            _cache.TryGetValue("GmailAccessToken", out string? token);

            if(token == null)
            {
                await RenewAccessToken();

                _cache.TryGetValue("GmailAccessToken", out string? newToken);

                return newToken;
            }
            return token;
        }
    }
}
