
using stok.Repository.Model.TokenManager;

namespace stok.Repository.Interace.Data.TokenManager
{
    public interface IRefreshTokenManagerData : IBaseData
    {
        Task<RefreshTokenManager> GetRefreshTokenByUserId(int userAccountId);
        Task<RefreshTokenManager> GetRefreshToken(string refreshToken);
    }
}
