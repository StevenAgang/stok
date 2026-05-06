using stok.Repository.Configurations;
using stok.Repository.Configurations.Exception_Extender;
using stok.Repository.Configurations.Validation;
using stok.Repository.Interace.Data.TokenManager;
using stok.Repository.Interace.Data.UserAccount;
using stok.Repository.Interace.TokenManager;
using stok.Repository.Interace.UserAccount;
using model = stok.Repository.Model.UserAccounts;
using stok.Repository.ViewModel.TokenManager;
using stok.Repository.ViewModel.UserAccount;
using users = stok.Repository.Model.UserAccounts;
using stok.Repository.Model.UserAccounts;
using stok.Repository.Interace.EmailService;
using stok.Repository.Model.TokenManager;

namespace stok.Repository.Service.UserAccount
{
    public class UserAccountService(
        IUserAccountData _userAccountData, 
        ITokenManagerService _tokenManager, 
        IRefreshTokenManagerService _refreshTokenManagerService,
        IRefreshTokenManagerData _refreshTokenManagerData,
        IEmailService _emailService,
        IForgotPasswordTokenManagerData _forgotPasswordTokenManagerData
        ) : IUserAccountService
    {

        public async Task<string[]> GmailSignin(UserAccountGoogleSignInViewModel userData)
        {
            var user = await _userAccountData.GetUserByEmailWithoutTracking(userData.Email, userData.Cancellation);

            if (user != null) 
            {

                var existingToken = await _refreshTokenManagerData.GetRefreshTokenByUserId(user.Id);
                
                if(existingToken!= null && existingToken.Platform != userData.Platform)
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
                    PositionType = user.PositionType == null ? "" : user.PositionType.Type
                };

                var jwtTokens = _tokenManager.GenerateJwtToken(jwts);

                return new string[] {jwtTokens, token.RefreshToken };
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
                Salt = null,
                IsActive = true,
                Created_At = DateTime.UtcNow
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

            string rawMail = _emailService.BuildRawMail(userData.Email,"Welcome to StokPH", _emailService.WelcomeUserMail());
            await _emailService.SendMail(rawMail);

            return new string[] {jwtToken, tokenData.RefreshToken};
        }

        public async Task Register(UserAccountRegistrationViewModel user)
        {
            UserAccountValidation.ValidInformation(user);
            UserAccountValidation.ValidEmailAndPassword(user);

            var existingUser = await _userAccountData.GetUserByEmailWithoutTracking(user.Email);

            if(existingUser != null)
            {
                throw new BadRequest($"{user.Email} is already associated with another account");
            }

            string salt = _tokenManager.GenerateSalt();
            var password = _tokenManager.Hashed(user.Password, salt);

            var info = new UserInformation
            {
                FirstName = user.FirstName,
                MiddleName = user.MidlleName,
                LastName = user.LastName,
            };

            await _userAccountData.Save(info);

            var newUser = new model.UserAccount
            {
                UserInformationId = info.Id,
                AccountTypeId = 3,
                PositionTypeId = null,
                Email = user.Email,
                PasswordHash = password,
                Salt = salt,
                IsActive = true,
                Created_At = DateTime.UtcNow
            };

            await _userAccountData.Save(newUser);

            string rawMail = _emailService.BuildRawMail(user.Email, "Welcome to StokPH", _emailService.WelcomeUserMail());
            await _emailService.SendMail(rawMail);
        }

        public async Task<UserAccountSuccessLoginViewModel> Login(UserAccountLoginViewModel user, CancellationToken cancellation)
        {
            UserAccountValidation.NotNullEmailAndPassword(user);

            var existingUser = await _userAccountData.GetUserByEmailWithoutTracking(user.Email);

            if(existingUser == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (existingUser.Disbaled == true && existingUser.DisabledTimer > DateTime.UtcNow)
            {
                throw new Forbidden($"Your account is disabled please login again after {existingUser.DisabledTimer} UTC time zone");
            }

            string password = _tokenManager.Hashed(user.Password, existingUser.Salt);

            if(password != existingUser.PasswordHash)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if(existingUser.Disbaled == true)
            {
                existingUser.Disbaled = false;
                existingUser.DisabledTimer = null;

                await _userAccountData.SaveChanges();
            }

            var existingToken = await _refreshTokenManagerData.GetRefreshTokenByUserId(existingUser.Id);

            if(existingToken != null && existingToken.Platform == user.Platform)
            {
                existingToken.Revoked = true;
                existingToken.RevocationReason = "User already log in on the same platform";
                existingToken.IsActive = false;
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.Updated_By = existingUser.Id;

                await _refreshTokenManagerData.SaveChanges();
            }

            var accountType = await _userAccountData.GetAccountTypeByUserId(existingUser.Id);

            var tokenData = new RefreshTokenManagerCreateViewModel
            {
                UserAccountId = existingUser.Id,
                RefreshToken = _tokenManager.GenerateRefreshToken(),
                UserAgent = user.UserAgent,
                Platform = user.Platform,
            };

            await _refreshTokenManagerService.Insert(tokenData, cancellation);

            string fullName = existingUser.UserInformation.FirstName + " " + (existingUser.UserInformation.MiddleName == "" ? "" : existingUser.UserInformation.MiddleName) + " " + existingUser.UserInformation.LastName;

            var jwt = new UserRoleAndPolicy
            {
                Id = existingUser.Id,
                FullName = fullName,
                AccountType = existingUser.AccountType.Type,
                PositionType = existingUser.PositionType == null ? "" : existingUser.PositionType.Type
            };

            var accessToken = _tokenManager.GenerateJwtToken(jwt);



            return new UserAccountSuccessLoginViewModel
            {
                Id = existingUser.Id,
                Fullname = fullName,
                AccessToken = accessToken,
                RefreshToken = tokenData.RefreshToken
            };
        }


        public async Task Recover(string email)
        {
            var existingUser = await _userAccountData.GetUserByEmailWithoutTracking(email);

            if (existingUser == null)
            {
                return;
            }

            var existingToken = await _forgotPasswordTokenManagerData.GetTokenByUserId(existingUser.Id);

            if (existingToken != null)
            {
                existingToken.IsActive = false;
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.Updated_By = existingUser.Id;

                await _forgotPasswordTokenManagerData.SaveChanges();
            }


            string token = _tokenManager.GenerateUrlToken();

            var fpEntity = new ForgotPasswordTokenManager
            {
                UserAccountId = existingUser.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsActive = true,
                Created_At = DateTime.UtcNow,
                Created_By = existingUser.Id
            };

            await _forgotPasswordTokenManagerData.Save(fpEntity);

            string link = $"https://localhost:4200/recovery/change-password?identity={token}";

            string rawMail = _emailService.BuildRawMail(existingUser.Email, "Recover your account", _emailService.RecoveryAccountMail(link));
            await _emailService.SendMail(rawMail);
            return;
        }


        public async Task ChangePassword(string token, string password)
        {
            UserAccountValidation.ValidPassword(password);
            var existingToken = await _forgotPasswordTokenManagerData.GetToken(token);

            if (existingToken == null) 
            {
                throw new BadRequest("This request is expired");
            }

            var existingUser = await _userAccountData.GetUserByIdWithTracking(existingToken.UserAccountId);

            if (DateTime.UtcNow > existingToken.ExpiresAt)
            {
                existingToken.IsActive = false;
                existingToken.Updated_At = DateTime.UtcNow;
                existingToken.Updated_By = existingUser.Id;

                await _forgotPasswordTokenManagerData.SaveChanges();
                throw new BadRequest();
            }

            existingToken.IsActive = false;
            existingToken.Updated_At = DateTime.UtcNow;
            existingToken.Updated_By = existingUser.Id;

            await _forgotPasswordTokenManagerData.SaveChanges();

            string salt = _tokenManager.GenerateSalt();
            string passwordHash = _tokenManager.Hashed(password, salt);


            existingUser.PasswordHash = passwordHash;
            existingUser.Salt = salt;
            existingUser.Updated_At = DateTime.UtcNow;
            existingUser.Updated_By = existingUser.Id;

            await _userAccountData.SaveChanges();

            return;
        }


        public async Task Logout(string refreshToken)
        {
            var token = await _refreshTokenManagerData.GetRefreshToken(refreshToken);

            if(token == null)
            {
                throw new Forbidden();
            }

            token.Revoked = true;
            token.RevocationReason = "User logged out";
            token.Updated_At = DateTime.UtcNow;
            token.IsActive = false;
            token.Updated_By = token.UserAccountId;
            await _refreshTokenManagerData.SaveChanges();
        }
    }
}
