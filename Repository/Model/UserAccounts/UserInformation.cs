namespace stok.Repository.Model.UserAccounts
{
    public sealed class UserInformation : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }

        public UserAccount? UserAccount { get; set; }
    }
}
