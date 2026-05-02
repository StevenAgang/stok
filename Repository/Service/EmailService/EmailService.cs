using stok.Repository.Interace.EmailService;
using stok.Repository.Interace.TokenManager;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace stok.Repository.Service.EmailService
{
    public class EmailService(IGmailTokenManagerService gmailTokenManager, IWebHostEnvironment env) : IEmailService
    {
        private readonly IGmailTokenManagerService _gmailTokenManager = gmailTokenManager;
        private readonly IWebHostEnvironment _env = env;

        public async Task SendMail(string rawMail)
        {
            string? token = await _gmailTokenManager.GetAccessToken();

            if (token == null) 
            {
                throw new Exception("Sending email failed");
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new {raw = rawMail };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://gmail.googleapis.com/gmail/v1/users/me/messages/send", content);
            response.EnsureSuccessStatusCode();
        }


        public string BuildRawMail(string to, string subject, string body)
        {

            var message =
                $"From: dev477601@gmail.com\r\n" +
                $"To: {to}\r\n" +
                $"Subject: {subject}\r\n" +
                $"Content-Type: text/html; charset=utf-8\r\n" +
                $"\r\n{body}";
            var bytes = Encoding.UTF8.GetBytes(message);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }


        public string RecoveryAccountMail(string resetLink)
        {
            var path = Path.Combine(_env.WebRootPath, "assets", "template", "recovery.html");

            string htmlBody = File.ReadAllText(path);
            htmlBody = htmlBody.Replace("{ResetLink}", resetLink);
            htmlBody = htmlBody.Replace("{YEAR}", DateTime.Now.Year.ToString());

            return htmlBody;
        }

        public string WelcomeUserMail()
        {
            var path = Path.Combine(_env.WebRootPath, "assets", "template", "welcome.html");

            string htmlBody = File.ReadAllText(path);
            htmlBody = htmlBody.Replace("{HOMELINK}", "https://localhost:4200/home");
            htmlBody = htmlBody.Replace("{CURRENTYEAR}", DateTime.Now.Year.ToString());
            htmlBody = htmlBody.Replace("{YEAR}", DateTime.Now.Year.ToString());

            return htmlBody;
        }
    }
}
