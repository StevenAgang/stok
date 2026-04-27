namespace stok.Repository.ViewModel.UserAccount
{
    public class UserAccountViewModel
    {
    }

    public class UserAccountRegistrationViewModel
    {
        public string? FirstName { get; set; }
        public string? MidlleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class UserAccountGoogleSignInViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserAgent { get; set; }
        public string? Platform { get; set; }
        public CancellationToken Cancellation { get; set; } = default;
    }

    public class UserRoleAndPolicy
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? AccountType { get; set; }
        public string? PositionType { get; set; }
    }
}
