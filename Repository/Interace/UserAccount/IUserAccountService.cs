using stok.Repository.ViewModel.UserAccount;

namespace stok.Repository.Interace.UserAccount
{
    public interface IUserAccountService
    {
        Task<string[]> GmailSignin(UserAccountGoogleSignInViewModel userData);
        Task Logout(int userAccountId);
    }
}
