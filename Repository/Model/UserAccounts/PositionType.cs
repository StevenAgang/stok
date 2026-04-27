namespace stok.Repository.Model.UserAccounts
{
    public sealed class PositionType : BaseEntity
    {
        public string? Type { get; set; }
        public ICollection<UserAccount> UserAccounts { get; set; } = [];
    }
}
