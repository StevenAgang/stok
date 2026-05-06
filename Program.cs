using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using stok.Middleware;
using stok.Repository;
using stok.Repository.Configurations.Helper;
using stok.Repository.Data;
using stok.Repository.Data.Scraping;
using stok.Repository.Data.TokenManager;
using stok.Repository.Data.UserAccount;
using stok.Repository.Interace.Data;
using stok.Repository.Interace.Data.Scraping;
using stok.Repository.Interace.Data.TokenManager;
using stok.Repository.Interace.Data.UserAccount;
using stok.Repository.Interace.EmailService;
using stok.Repository.Interace.Scraping;
using stok.Repository.Interace.TokenManager;
using stok.Repository.Interace.UserAccount;
using stok.Repository.Service.EmailService;
using stok.Repository.Service.Hubs;
using stok.Repository.Service.Scraping;
using stok.Repository.Service.TokenManager;
using stok.Repository.Service.UserAccount;
using System.Security.Claims;

namespace stok
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontEnd",policy =>
                {
                    policy.WithOrigins("https://localhost:4200", "http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            #region Scoped Services
            // Services
            //builder.Services.AddScoped<IScrapingService, ScrapingService>();
            builder.Services.AddScoped<IUserAccountService,UserAccountService>();
            builder.Services.AddScoped<IRefreshTokenManagerService, RefreshTokenManagerService>();
            builder.Services.AddScoped<IScrapingService, ScrapingService>();

            // Data Access
            builder.Services.AddScoped<IBaseData, BaseData>();
            builder.Services.AddScoped<IUserAccountData, UserAccountData>();
            builder.Services.AddScoped<IRefreshTokenManagerData, RefreshTokenManagerData>();
            builder.Services.AddScoped<IForgotPasswordTokenManagerData, ForgotPasswordTokenManagerData>();
            builder.Services.AddScoped<IScrapingData, ScrapingData>();
            #endregion

            #region Singleton Services

            builder.Services.AddSingleton<ResponseHelper>();
            builder.Services.AddSingleton<ITokenManagerService, TokenManagerService>();
            builder.Services.AddSingleton<IGmailTokenManagerService, GmailTokenManagerService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();

            #endregion

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    RoleClaimType = ClaimTypes.Role

                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.HttpContext.Request.Cookies["AccessToken"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie()
            .AddGoogle("GoogleDefault", options =>
            {
                options.ClientId = builder.Configuration["GoogleAuthDefault:ClientId"];
                options.ClientSecret = builder.Configuration["GoogleAuthDefault:ClientSecret"];

                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("openid");
                options.SaveTokens = true;
                options.AccessType = "offline";
            });
            //.AddGoogle("GoogleAdmin", options =>
            //{
            //    options.ClientId = builder.Configuration["GoogleAuthAdmin:ClientId"];
            //    options.ClientSecret = builder.Configuration["GoogleAuthAdmin:ClientSecret"];

            //    options.Scope.Add("profile");
            //    options.Scope.Add("email");
            //    options.Scope.Add("openid");
            //    options.Scope.Add("https://www.googleapis.com/auth/gmail.send");
            //    options.SaveTokens = true;
            //    options.AccessType = "offline";
            //    options.Events.OnRedirectToAuthorizationEndpoint = context =>
            //    {
            //        context.Response.Redirect(context.RedirectUri + "&prompt=consent");
            //        return Task.CompletedTask;
            //    };

            //    //options.CallbackPath = "/user/google-admin-callback";
            //});

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTeamLead", policy =>
                {
                    policy.RequireClaim("Position", "TL");
                });
            });

            builder.Services.AddDbContext<DatabaseContext>(context => context.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSignalR();

            builder.Services.AddMemoryCache();

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            app.UseCors("FrontEnd");

            if (app.Environment.IsDevelopment()) 
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequestMiddleware>();
            app.MapHub<ScrapingHubService>("/scrapinghub");
            app.MapControllers();

            app.Run();
        }
    }
} 
