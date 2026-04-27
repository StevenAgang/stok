using stok.Repository.Configurations;
using stok.Repository.ViewModel.UserAccount;

namespace stok.Repository.Interace.TokenManager
{
    public interface ITokenManagerService
    {
        string GenerateJwtToken(UserRoleAndPolicy roles);
        HttpContext SetAccessTokenCookie(string token, HttpContextSetting httpContextSetting, HttpContext context);
        HttpContext SetRefreshTokenCookie(string token, HttpContextRefreshTokenSettings httpContextRefreshTokenSettings, HttpContext context);
        string GenerateRefreshToken();
        string Hashed<T>(T value, string salt);
        string GenerateSalt();
    }
}
