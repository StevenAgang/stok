using stok.Middleware;
using stok.Repository.Configurations.Helper;
using stok.Repository.Interace.Scraping;
using stok.Repository.Service.Scraping;
using System.Net;

namespace stok
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            #region Scoped Services
            builder.Services.AddScoped<IScrapingService, ScrapingService>();
            #endregion

            #region Singleton Services

            builder.Services.AddSingleton<ResponseHelper>();
            builder.Services.AddSingleton<HttpClient>(c =>
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer(),
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                   "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                   "AppleWebKit/537.36 (KHTML, like Gecko) " +
                   "Chrome/123.0 Safari/537.36");
                client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");

                return client;
            });

            #endregion

            builder.Services.AddHttpClient();

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) 
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure the HTTP request pipeline.

            app.UseMiddleware<RequestMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
