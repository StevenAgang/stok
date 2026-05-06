using ac = stok.Repository.Model.UserAccounts;
namespace stok.Repository.Model.TokenManager
{
    public class ScrapeServiceTokenManager : BaseEntity
    {
        public int UserAccountId { get; set; }
        public ac.UserAccount? UserAccount { get; set; }
        public string? Token { get; set; }
    }
}
