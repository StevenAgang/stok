using stok.Repository.Model.UserAccounts;
using user = stok.Repository.Model.UserAccounts;
namespace stok.Repository.Interace.Data.UserAccount
{
    public interface IUserAccountData : IBaseData
    {
        Task<user.UserAccount> GetUserByEmailWithoutTracking(string email, CancellationToken cancellation = default);
        Task<user.UserAccount> GetUserByEmailWithTracking(string email, CancellationToken cancellation = default);
        Task<user.UserAccount> GetUserByIdWithoutTracking(int userId, CancellationToken cancellation = default);
        Task<user.UserAccount> GetUserByIdWithTracking(int userId, CancellationToken cancellation = default);
        Task<user.UserAccount> GetUserInformationByUserIdWithTracking(int userId, CancellationToken cancellation = default);
        Task<user.UserAccount> GetUserInformationByUserIdWithoutTracking(int userId, CancellationToken cancellation = default);
        Task<AccountType> GetAccountTypeByUserId(int accountTypeId, CancellationToken cancellation = default);
    }
}
