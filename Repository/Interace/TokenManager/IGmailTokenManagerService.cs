namespace stok.Repository.Interace.TokenManager
{
    public interface IGmailTokenManagerService
    {
        Task<string?> GetAccessToken();
    }
}
