using Microsoft.IdentityModel.Tokens;
using stok.Repository.Configurations;
using stok.Repository.Interace.TokenManager;
using stok.Repository.ViewModel.TokenManager;
using stok.Repository.ViewModel.UserAccount;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace stok.Repository.Service.TokenManager
{
    public class TokenManagerService(IConfiguration config) : ITokenManagerService
    {
        private readonly IConfiguration _config = config;

        public string GenerateJwtToken(UserRoleAndPolicy roles)
        {
            var jwtSetting = new JwtSetting();
            _config.GetSection("Jwt").Bind(jwtSetting);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, roles.Id.ToString()),
                new (JwtRegisteredClaimNames.Name, roles.FullName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new ("Role", roles.AccountType),
                new ("Position", roles.PositionType)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSetting.ExpireInMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public HttpContext SetAccessTokenCookie(string token, HttpContextSetting httpContextSetting, HttpContext context)
        {
            context.Response.Cookies.Append("AccessToken", token, new CookieOptions
            {
                HttpOnly = httpContextSetting.IsHttpOnly,
                Secure = httpContextSetting.IsSecure,
                SameSite = Enum.TryParse<SameSiteMode>(httpContextSetting.SameSite, out var mode) ? mode : SameSiteMode.Lax,
                //Expires = DateTime.UtcNow.AddMinutes(httpContextSetting.ExpireInMinutes)
                Expires = DateTime.UtcNow.AddSeconds(10)
            });
            return context;
        }

        public HttpContext SetRefreshTokenCookie(string token, HttpContextRefreshTokenSettings httpContextRefreshTokenSettings, HttpContext context)
        {
            context.Response.Cookies.Append("RefreshToken", token, new CookieOptions
            {
                HttpOnly = httpContextRefreshTokenSettings.IsHttpOnly,
                Secure = httpContextRefreshTokenSettings.IsSecure,  
                SameSite = Enum.TryParse<SameSiteMode>(httpContextRefreshTokenSettings.SameSite, out var mode) ? mode : SameSiteMode.Lax,
                //Expires = DateTime.UtcNow.AddDays(httpContextRefreshTokenSettings.ExpireInDays)
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return context;
        }

        public string GenerateRefreshToken()
        {
            var random = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(random);  
        }

        public string Hashed(string value, string salt)
        {
            var combined = Encoding.UTF8.GetBytes(value + salt);
            var hash = SHA256.HashData(combined);
            var base64 = Convert.ToBase64String(hash);

            return base64;
        }

        public string GenerateUrlToken()
        {
            var random = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(random).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public HttpContext FinalizeToken(FinalizeTokenViewModel token)
        {
            var httpContextSetting = new HttpContextSetting();
            var HttpContextRefreshTokenSettings = new HttpContextRefreshTokenSettings();
            _config.GetSection("HttpContextSettings").Bind(httpContextSetting);
            _config.GetSection("HttpContextRefreshTokenSettings").Bind(HttpContextRefreshTokenSettings);

            SetAccessTokenCookie(token.AccessToken, httpContextSetting, token.Context);
            SetRefreshTokenCookie(token.RefreshToken, HttpContextRefreshTokenSettings, token.Context);

            return token.Context;
        }

        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
