using stok.Repository.ViewModel.TokenManager;

namespace stok.Repository.Interace.TokenManager
{
    public interface IRefreshTokenManagerService
    {
        Task Insert(RefreshTokenManagerCreateViewModel refreshToken, CancellationToken cancellation = default);
        Task<string[]> Refresh(RefreshTokenManagerCreateViewModel refreshToken, CancellationToken cancellation = default);
    }
}
