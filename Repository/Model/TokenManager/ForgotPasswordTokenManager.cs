using stok.Repository.Model.UserAccounts;

namespace stok.Repository.Model.TokenManager
{
    public class ForgotPasswordTokenManager : BaseEntity
    {
        public int UserAccountId { get; set; }
        public UserAccount? UserAccount { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
