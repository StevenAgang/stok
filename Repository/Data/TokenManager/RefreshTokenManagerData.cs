using Microsoft.EntityFrameworkCore;
using stok.Repository.Interace.Data.TokenManager;
using stok.Repository.Model.TokenManager;

namespace stok.Repository.Data.TokenManager
{
    public class RefreshTokenManagerData(DatabaseContext context) : BaseData(context), IRefreshTokenManagerData
    {
        public async Task<RefreshTokenManager> GetRefreshTokenByUserId(int userAccountId)
        {
            var token = await BaseQuery<RefreshTokenManager>(true).FirstOrDefaultAsync(t => t.UserAccountId == userAccountId && t.Revoked == false && t.IsActive == true);
            return token;
        }
    }
}
