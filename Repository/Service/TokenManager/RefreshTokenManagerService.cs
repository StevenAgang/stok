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
        IRefreshTokenManagerData _refreshTokenManagerData, 
        ITokenManagerService _tokenManager, 
        IUserAccountData _userAccount
        ) : IRefreshTokenManagerService
    {
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

           await _refreshTokenManagerData.Save(token, cancellation);
        }

        public async Task<string[]> Refresh(RefreshTokenUpdateViewModel refreshToken, CancellationToken cancellation = default)
        {

            RefreshTokenValidation.ValidateRefreshToken(refreshToken.RefreshToken);

            var existingToken = await _refreshTokenManagerData.GetRefreshToken(refreshToken.RefreshToken);

            if (existingToken == null || existingToken.RefreshToken != refreshToken.RefreshToken)
            {
                throw new Forbidden("Forbidden request");
            }


            if(existingToken.Platform != refreshToken.Platform)
            {
                existingToken.Revoked = true;
                existingToken.RevocationReason = "Platform mismatch";
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.IsActive = false;
                existingToken.Updated_By = existingToken.UserAccountId;
                await _refreshTokenManagerData.SaveChanges(cancellation);
                throw new Forbidden("Forbidden request");
            }

            if(DateTime.UtcNow > existingToken.ExpiresAt)
            {
                existingToken.Revoked = true;
                existingToken.RevocationReason = "Refresh token expired";
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.IsActive = false;
                existingToken.Updated_By = existingToken.UserAccountId;
                await _refreshTokenManagerData.SaveChanges(cancellation);
                throw new Forbidden("Forbidden request");
            }

            existingToken.Revoked = true;
            existingToken.RevocationReason = "Token refreshed";
            existingToken.Updated_At = DateTime.UtcNow;
            existingToken.IsActive = false;
            existingToken.Updated_By = existingToken.UserAccountId;

            await _refreshTokenManagerData.SaveChanges(cancellation);

            var token = await IssueToken(existingToken.UserAccountId);

            var createToken = new RefreshTokenManagerCreateViewModel
            {
                UserAccountId = existingToken.UserAccountId,
                RefreshToken = token[1],
                UserAgent = refreshToken.UserAgent,
                Platform = refreshToken.Platform
            };

            await Insert(createToken, cancellation);

            return token;
        }

        private async Task<string[]> IssueToken(int UserAccountId)
        {
            var user = await _userAccount.GetUserInformationByUserIdWithoutTracking(UserAccountId);
            string fullName = user.UserInformation.FirstName + (string.IsNullOrEmpty(user.UserInformation.MiddleName) ? "" : " " + user.UserInformation.MiddleName) + " " + user.UserInformation.LastName;

            var jwt = new UserRoleAndPolicy
            {
                Id = user.Id,
                FullName = fullName,
                AccountType = user.AccountType.Type,
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
