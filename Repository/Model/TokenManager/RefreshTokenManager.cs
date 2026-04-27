using stok.Repository.Model.UserAccounts;

namespace stok.Repository.Model.TokenManager
{
    public class RefreshTokenManager : BaseEntity
    {
        public int UserAccountId { get; set; }
        public UserAccount? UserAccount { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }
        public string? RevocationReason { get; set; }
        public string? UserAgent { get; set; }
        public string? Platform { get; set; }
    }
}
