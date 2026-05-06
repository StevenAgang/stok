using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using stok.Repository.Configurations;
using stok.Repository.Configurations.Attribute_Extender;
using stok.Repository.Configurations.Helper;
using stok.Repository.Interace.TokenManager;
using stok.Repository.Interace.UserAccount;
using stok.Repository.ViewModel.TokenManager;
using stok.Repository.ViewModel.UserAccount;
using System.Security.Claims;

namespace stok.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserAccountController(
        IUserAccountService _userAccount, 
        IConfiguration _config, 
        ITokenManagerService _tokenManager,
        IRefreshTokenManagerService _refresTokenManager,
        ResponseHelper _response
        ) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAccountLoginViewModel user, CancellationToken cancellation)
        {
            var result = await _userAccount.Login(user, cancellation);

            var finalize = new FinalizeTokenViewModel
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                Context = HttpContext
            };

            _tokenManager.FinalizeToken(finalize);

            result.AccessToken = "";
            result.RefreshToken = "";

            return StatusCode(200, _response.Status(200, true, $"Welcome back {result.Fullname}"));
        }


        [Transaction]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAccountRegistrationViewModel user)
        {
            await _userAccount.Register(user);

            return StatusCode(200, _response.Status(200, true, "Account created successfully"));
        }

        [HttpGet("recover")]
        public async Task<IActionResult> Recover([FromQuery] string email)
        {
            await _userAccount.Recover(email);
            return StatusCode(200, _response.Status(200, true, "Instruction sent"));
        }

        [HttpPatch("recover/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel password, [FromQuery] string token)
        {
            await _userAccount.ChangePassword(token, password.Password);
            return StatusCode(200, _response.Status(200, true, "Change Password Successfully"));
        }

        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn([FromQuery] string userAgent, [FromQuery] string platform)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = $"/user/google-callback?userAgent={userAgent}&platform={platform}" }, "GoogleDefault");
        }

        [Transaction]
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallBack([FromQuery] string userAgent, [FromQuery] string platform)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var firstName = result.Principal?.FindFirst(ClaimTypes.Surname)?.Value;
            var lastName = result.Principal?.FindFirst(ClaimTypes.GivenName)?.Value;
            var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            var GoogleSignRedirectSuccess = new GoogleSignRedirectSuccess();

            _config.GetSection("GoogleSignRedirectSuccess").Bind(GoogleSignRedirectSuccess);

            if(GoogleSignRedirectSuccess.RedirectUri == null)
            {
                throw new Exception("Redirect URI propety is not set");
            }

            var userData = new UserAccountGoogleSignInViewModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserAgent = userAgent,
                Platform = platform,
            };

            var signinResult = await _userAccount.GmailSignin(userData);


            var finalize = new FinalizeTokenViewModel
            {
                AccessToken = signinResult[0],
                RefreshToken = signinResult[1],
                Context = HttpContext
            };

            _tokenManager.FinalizeToken(finalize);

            return Redirect(GoogleSignRedirectSuccess.RedirectUri);
        }

        //[HttpGet("google-admin-signin")]
        //public IActionResult GoogleAdminSignIn()
        //{
        //    return Challenge(new AuthenticationProperties { RedirectUri = "/user/google-admin-callback" }, "GoogleAdmin");
        //}

        //[HttpGet("google-admin-callback")]
        //public async Task<IActionResult> GoogleAdminCallBack()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    if (!result.Succeeded)
        //    {
        //        return Unauthorized();
        //    }
        //    var accessToken = result.Properties.GetTokenValue("access_token");
        //    var refreshToken = result.Properties.GetTokenValue("refresh_token");
        //    var name = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;
        //    var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        //    return Redirect("http://localhost:4200/login");
        //}

        [HttpPatch("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenUpdateViewModel token)
        {

            string refreshToken = HttpContext.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return StatusCode(403, _response.Status(403, false, null));
            }

            token.RefreshToken = refreshToken;

            var result = await _refresTokenManager.Refresh(token);

            var finalize = new FinalizeTokenViewModel
            {
                AccessToken = result[0],
                RefreshToken = result[1],
                Context = HttpContext
            };

            _tokenManager.FinalizeToken(finalize);

            return StatusCode(200, _response.Status(200, true, "Updated Successfully"));
        }

    }
}
