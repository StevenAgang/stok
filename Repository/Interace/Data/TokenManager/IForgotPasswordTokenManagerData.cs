using stok.Repository.Model.TokenManager;

namespace stok.Repository.Interace.Data.TokenManager
{
    public interface IForgotPasswordTokenManagerData : IBaseData
    {
        Task<ForgotPasswordTokenManager> GetToken(string token);
        Task<ForgotPasswordTokenManager> GetTokenByUserId(int userId);
    }
}
