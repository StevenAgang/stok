using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
        IUserAccountService userAccount, 
        IConfiguration config, 
        ITokenManagerService tokenManager,
        IRefreshTokenManagerService refresTokenManager,
        ResponseHelper response
        ) : ControllerBase
    {
        private readonly IUserAccountService _userAccount = userAccount;
        private readonly IConfiguration _config = config;
        private readonly ITokenManagerService _tokenManager = tokenManager;
        private readonly IRefreshTokenManagerService _refresTokenManager = refresTokenManager;
        private readonly ResponseHelper _response = response;

        [Authorize]
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserAccountRegistrationViewModel user)
        {
            throw new NotImplementedException();
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

            var httpContextSetting = new HttpContextSetting();
            var HttpContextRefreshTokenSettings = new HttpContextRefreshTokenSettings();

            _config.GetSection("HttpContextSettings").Bind(httpContextSetting);
            _config.GetSection("HttpContextRefreshTokenSettings").Bind(HttpContextRefreshTokenSettings);

            var userData = new UserAccountGoogleSignInViewModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserAgent = userAgent,
                Platform = platform,
            };

            var signinResult = await _userAccount.GmailSignin(userData);

            _tokenManager.SetAccessTokenCookie(signinResult[1], httpContextSetting, HttpContext);
            _tokenManager.SetRefreshTokenCookie(signinResult[0], HttpContextRefreshTokenSettings, HttpContext);

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
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenManagerCreateViewModel token)
        {

            string refreshToken = HttpContext.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                await _userAccount.Logout(token.UserAccountId);
            }

            token.RefreshToken = refreshToken;

            var result = await _refresTokenManager.Refresh(token);

            var httpContextSetting = new HttpContextSetting();
            var HttpContextRefreshTokenSettings = new HttpContextRefreshTokenSettings();

            _config.GetSection("HttpContextSettings").Bind(httpContextSetting);
            _config.GetSection("HttpContextRefreshTokenSettings").Bind(HttpContextRefreshTokenSettings);

            _tokenManager.SetAccessTokenCookie(result[0], httpContextSetting, HttpContext);
            _tokenManager.SetRefreshTokenCookie(result[1], HttpContextRefreshTokenSettings, HttpContext);

            return StatusCode(200, _response.Status(200, true, "Updated Successfully"));
        }

    }
}
