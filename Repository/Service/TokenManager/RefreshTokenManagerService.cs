using stok.Repository.Configurations.Exception_Extender;
using stok.Repository.Configurations.Validation;
using stok.Repository.Interace.Data.TokenManager;
using stok.Repository.Interace.Data.UserAccount;
using stok.Repository.Interace.TokenManager;
using stok.Repository.Model.TokenManager;
using stok.Repository.Model.UserAccounts;
using stok.Repository.ViewModel.TokenManager;
using stok.Repository.ViewModel.UserAccount;

namespace stok.Repository.Service.TokenManager
{
    public class RefreshTokenManagerService(
        IRefreshTokenManagerData refreshTokenManagerData, 
        ITokenManagerService tokenManager, 
        IUserAccountData userAccount
        ) : IRefreshTokenManagerService
    {
        private readonly IRefreshTokenManagerData _refreshTokenManagerData = refreshTokenManagerData;
        private readonly ITokenManagerService _tokenManager = tokenManager;
        private readonly IUserAccountData _userAccount = userAccount;
        public async Task Insert(RefreshTokenManagerCreateViewModel refreshToken, CancellationToken cancellation = default)
        {
            var token = new RefreshTokenManager
            {
                UserAccountId = refreshToken.UserAccountId,
                RefreshToken = refreshToken.RefreshToken,
                UserAgent = refreshToken.UserAgent,
                Platform = refreshToken.Platform,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Set expiration time as needed
                Revoked = false,
                IsActive = true,
                Created_At = DateTime.UtcNow,
                Created_By = refreshToken.UserAccountId

            };

           await refreshTokenManagerData.Save(token, cancellation);
        }

        public async Task<string[]> Refresh(RefreshTokenManagerCreateViewModel refreshToken, CancellationToken cancellation = default)
        {
            if(refreshToken.UserAccountId == null || refreshToken.UserAccountId <= 0)
            {
                 throw new Forbidden("Forbidden request");
            }

            var existingToken = await _refreshTokenManagerData.GetRefreshTokenByUserId(refreshToken.UserAccountId);

            RefreshTokenValidation.ValidateRefreshToken(refreshToken.RefreshToken);

            if (existingToken.RefreshToken != refreshToken.RefreshToken)
            {
                throw new Forbidden("Forbidden request");
            }

            if(existingToken.Platform != refreshToken.Platform)
            {
                existingToken.Revoked = true;
                existingToken.RevocationReason = "Platform mismatch";
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.IsActive = false;
                existingToken.Updated_By = refreshToken.UserAccountId;
                await _refreshTokenManagerData.SaveChanges(cancellation);
                throw new Forbidden("Forbidden request");
            }

            if(DateTime.UtcNow > existingToken.ExpiresAt)
            {
                existingToken.Revoked = true;
                existingToken.RevocationReason = "Refresh token expired";
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.IsActive = false;
                existingToken.Updated_By = refreshToken.UserAccountId;
                await _refreshTokenManagerData.SaveChanges(cancellation);
                throw new Forbidden("Forbidden request");
            }

            existingToken.Revoked = true;
            existingToken.RevocationReason = "Token refreshed";
            existingToken.Updated_At = DateTime.UtcNow;
            existingToken.IsActive = false;
            existingToken.Updated_By = refreshToken.UserAccountId;

            await _refreshTokenManagerData.SaveChanges(cancellation);

            var token = await IssueToken(refreshToken.UserAccountId);

            refreshToken.RefreshToken = token[1]; // Generate a new refresh token

            await Insert(refreshToken, cancellation);

            return token;
        }

        private async Task<string[]> IssueToken(int UserAccountId)
        {
            var accessType = await _userAccount.GetAccountTypeByUserId(UserAccountId);
            var user = await _userAccount.GetUserInformationByUserIdWithoutTracking(UserAccountId);
            string fullName = user.UserInformation.FirstName + (string.IsNullOrEmpty(user.UserInformation.MiddleName) ? "" : " " + user.UserInformation.MiddleName) + " " + user.UserInformation.LastName;

            var jwt = new UserRoleAndPolicy
            {
                Id = user.Id,
                FullName = fullName,
                AccountType = accessType.Type,
                PositionType = user.PositionTypeId == null ? "" : user.PositionType.Type
            };

            var jwtToken = _tokenManager.GenerateJwtToken(jwt);

            return new string[]
            {
                jwtToken,
                _tokenManager.GenerateRefreshToken()
            };

        }
    }
}
