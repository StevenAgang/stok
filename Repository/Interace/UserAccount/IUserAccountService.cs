using stok.Repository.ViewModel.UserAccount;

namespace stok.Repository.Interace.UserAccount
{
    public interface IUserAccountService
    {
        Task<string[]> GmailSignin(UserAccountGoogleSignInViewModel userData);
        Task Logout(string refreshToken);
        Task<UserAccountSuccessLoginViewModel> Login(UserAccountLoginViewModel user, CancellationToken cancellation);
        Task Register(UserAccountRegistrationViewModel user);
        Task Recover(string email);
        Task ChangePassword(string token, string password);
    }
}
