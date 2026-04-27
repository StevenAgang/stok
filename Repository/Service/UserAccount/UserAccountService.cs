using stok.Repository.Configurations;
using stok.Repository.Configurations.Exception_Extender;
using stok.Repository.Interace.Data.TokenManager;
using stok.Repository.Interace.Data.UserAccount;
using stok.Repository.Interace.TokenManager;
using stok.Repository.Interace.UserAccount;
using stok.Repository.ViewModel.TokenManager;
using stok.Repository.ViewModel.UserAccount;
using users = stok.Repository.Model.UserAccounts;

namespace stok.Repository.Service.UserAccount
{
    public class UserAccountService(
        IUserAccountData userAccountData, 
        ITokenManagerService tokenManager, 
        IRefreshTokenManagerService refreshTokenManagerService,
        IRefreshTokenManagerData refreshTokenManagerData
        ) : IUserAccountService
    {
        private readonly IUserAccountData _userAccountData = userAccountData;
        private readonly ITokenManagerService _tokenManager = tokenManager;
        private readonly IRefreshTokenManagerService _refreshTokenManagerService = refreshTokenManagerService;
        private readonly IRefreshTokenManagerData _refreshTokenManagerData = refreshTokenManagerData;

        public async Task<string[]> GmailSignin(UserAccountGoogleSignInViewModel userData)
        {
            var user = await _userAccountData.GetUserByEmailWithoutTracking(userData.Email, userData.Cancellation);

            if (user != null) 
            {

                var existingToken = await _refreshTokenManagerData.GetRefreshTokenByUserId(user.Id);
                
                if(existingToken.Platform != userData.Platform)
                {
                    existingToken.Revoked = true;
                    existingToken.RevocationReason = "Multiple request of refresh token from the same type of device";
                    existingToken.IsActive = false;
                    existingToken.Updated_At = DateTime.UtcNow;
                    existingToken.Updated_By = user.Id;

                    await _refreshTokenManagerData.Save(existingToken);
                }

                var token = new RefreshTokenManagerCreateViewModel
                {
                    UserAccountId = user.Id,
                    RefreshToken = _tokenManager.GenerateRefreshToken(),
                    UserAgent = userData.UserAgent,
                    Platform = userData.Platform,
                };

                string name = user.UserInformation.FirstName + " " + (string.IsNullOrWhiteSpace(user.UserInformation.MiddleName) ? "" : user.UserInformation.MiddleName) + " " + user.UserInformation.LastName;

                await _refreshTokenManagerService.Insert(token);

                var type = await _userAccountData.GetAccountTypeByUserId(user.AccountTypeId, userData.Cancellation);

                var jwts = new UserRoleAndPolicy
                {
                    Id = user.Id,
                    FullName = name,
                    AccountType = type.Type,
                    PositionType = user.PositionType.Type == null ? "" : user.PositionType.Type
                };

                var jwtTokens = _tokenManager.GenerateJwtToken(jwts);

                return new string[] { token.RefreshToken, jwtTokens };
            }
            var info = new users.UserInformation
            {
                FirstName = userData.FirstName,
                LastName = userData.LastName
            };

            await _userAccountData.Save(info, userData.Cancellation);

            var newUser = new users.UserAccount
            {
                UserInformationId = info.Id,
                AccountTypeId = 3,
                PositionTypeId = null,
                Email = userData.Email,
                PasswordHash = null,
                Salt = null
            };

            await _userAccountData.Save(newUser, userData.Cancellation);

            var tokenData = new RefreshTokenManagerCreateViewModel
            {
                UserAccountId = newUser.Id,
                RefreshToken = _tokenManager.GenerateRefreshToken(),
                UserAgent = userData.UserAgent,
                Platform = userData.Platform,
            };

            string fullName = info.FirstName + " " + (string.IsNullOrWhiteSpace(info.MiddleName) ? "" : info.MiddleName) + " " + info.LastName;

            await _refreshTokenManagerService.Insert(tokenData);

            var accountType = await _userAccountData.GetAccountTypeByUserId(newUser.AccountTypeId, userData.Cancellation);

            var jwt = new UserRoleAndPolicy
            {
                Id = newUser.Id,
                FullName = fullName,
                AccountType = accountType.Type,
                PositionType = ""
            };

            var jwtToken = _tokenManager.GenerateJwtToken(jwt);

            return new string[] { tokenData.RefreshToken, jwtToken};
        }


        public async Task Logout(int userAccountId)
        {
            var token = await _refreshTokenManagerData.GetRefreshTokenByUserId(userAccountId);

            if(token == null)
            {
                throw new NotFound();
            }

            token.Revoked = true;
            token.RevocationReason = "User logged out";
            token.Updated_At = DateTime.UtcNow;
            token.IsActive = false;
            token.Updated_By = userAccountId;
            await _refreshTokenManagerData.SaveChanges();
        }
    }
}
