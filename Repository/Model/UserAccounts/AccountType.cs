namespace stok.Repository.Model.UserAccounts
{
    public sealed class AccountType : BaseEntity
    {
        public string? Type { get; set; }
        public ICollection<UserAccount> UserAccounts { get; set; } = [];
    }
}
