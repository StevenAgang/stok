using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stok.Repository.Configurations.Helper;
using System.Security.Claims;

namespace stok.Controllers.ViewAccess
{
    [ApiController]
    public class ViewAccessController(ResponseHelper response) : ControllerBase
    {
        private readonly ResponseHelper _response = response;

        [Authorize]
        [HttpGet("authorize")]
       public IActionResult Authorize()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.FindFirst("name")?.Value;
            var position = User.FindFirst("position")?.Value;
            return StatusCode(200, _response.Status(200, true, null, new string[] { id, name, position }));
        }
    }
}
