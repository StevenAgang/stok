namespace stok.Repository.ViewModel.TokenManager
{
    public class RefreshTokenManagerViewModel
    {
    }
    
    public class RefreshTokenManagerCreateViewModel 
    {
        public int UserAccountId { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserAgent { get; set; }
        public string? Platform { get; set; }
    }

    public class RefreshTokenUpdateViewModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
