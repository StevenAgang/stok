using Microsoft.EntityFrameworkCore;
using stok.Repository.Interace.Data.TokenManager;
using stok.Repository.Model.TokenManager;

namespace stok.Repository.Data.TokenManager
{
    public class ForgotPasswordTokenManagerData(DatabaseContext context) : BaseData(context), IForgotPasswordTokenManagerData
    {
        public async Task<ForgotPasswordTokenManager> GetToken(string token)
        {
            var fpToken = await BaseQuery<ForgotPasswordTokenManager>(true).FirstOrDefaultAsync(t => t.Token == token && t.IsActive == true);
            return fpToken;
        }
        public async Task<ForgotPasswordTokenManager> GetTokenByUserId(int userId)
        {
            var fpToken = await BaseQuery<ForgotPasswordTokenManager>(true).FirstOrDefaultAsync(t => t.UserAccountId == userId && t.IsActive == true);
            return fpToken;
        }
    }
}
