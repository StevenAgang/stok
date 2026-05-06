using Microsoft.EntityFrameworkCore;
using stok.Repository.Interace.Data;
using stok.Repository.Interace.Data.UserAccount;
using stok.Repository.Model.UserAccounts;
using user = stok.Repository.Model.UserAccounts;
namespace stok.Repository.Data.UserAccount
{
    public class UserAccountData(DatabaseContext context) : BaseData(context), IUserAccountData
    {

        public async Task<user.UserAccount> GetUserByEmailWithoutTracking(string email, CancellationToken cancellation = default)
        {
            var user =  await BaseQuery<user.UserAccount>(false).Include(u => u.UserInformation).Include(p => p.PositionType).Include(a => a.AccountType).Include(s => s.ScrapeServiceTokenManager).FirstOrDefaultAsync(x => x.Email == email, cancellation);
            return user;
        }

        public async Task<user.UserAccount> GetUserByEmailWithTracking(string email, CancellationToken cancellation = default)
        {
            var user = await BaseQuery<user.UserAccount>(true).Include(u => u.UserInformation).Include(p => p.PositionType).Include(a => a.AccountType).Include(s => s.ScrapeServiceTokenManager).FirstOrDefaultAsync(x => x.Email == email, cancellation);
            return user;
        }
        
        public async Task<AccountType> GetAccountTypeByUserId(int accountTypeId, CancellationToken cancellation = default)
        {
            var type = await BaseQuery<AccountType>(false).FirstOrDefaultAsync(x => x.Id == accountTypeId, cancellation);
            return type;
        }

        public async Task<user.UserAccount> GetUserByIdWithoutTracking(int userId, CancellationToken cancellation = default)
        {
            var user = await BaseQuery<user.UserAccount>(false).Include(u => u.UserInformation).Include(p => p.PositionType).Include(a => a.AccountType).Include(s => s.ScrapeServiceTokenManager).FirstOrDefaultAsync(x => x.Id == userId, cancellation);
            return user;
        }

        public async Task<user.UserAccount> GetUserByIdWithTracking(int userId, CancellationToken cancellation = default)
        {
            var user = await BaseQuery<user.UserAccount>(true).Include(u => u.UserInformation).Include(p => p.PositionType).Include(a => a.AccountType).Include(s => s.ScrapeServiceTokenManager).FirstOrDefaultAsync(x => x.Id == userId, cancellation);
            return user;
        }

        public async Task<user.UserAccount> GetUserInformationByUserIdWithTracking(int userId, CancellationToken cancellation = default)
        {
            var user = await BaseQuery<user.UserAccount>(true).Include(x => x.UserInformation).Include(p => p.PositionType).Include(a => a.AccountType).Include(s => s.ScrapeServiceTokenManager).FirstOrDefaultAsync(x => x.Id == userId, cancellation);
            return user;
        }
        public async Task<user.UserAccount> GetUserInformationByUserIdWithoutTracking(int userId, CancellationToken cancellation = default)
        {
            var user = await BaseQuery<user.UserAccount>(false).Include(x => x.UserInformation).Include(p => p.PositionType).Include(a => a.AccountType).Include(s => s.ScrapeServiceTokenManager).FirstOrDefaultAsync(x => x.Id == userId, cancellation);
            return user;
        }

    }
}
