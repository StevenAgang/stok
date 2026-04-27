using stok.Repository.Model.TokenManager;
using System.ComponentModel.DataAnnotations.Schema;

namespace stok.Repository.Model.UserAccounts
{
    public sealed class UserAccount : BaseEntity
    {
        public int UserInformationId { get; set; }
        public UserInformation? UserInformation { get; set; }
        public int AccountTypeId { get; set; }
        public AccountType? AccountType { get; set; }
        public int? PositionTypeId { get; set; }
        public PositionType? PositionType { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }

        public ICollection<RefreshTokenManager> RefreshTokenManager { get; set; } = [];
    }
}
