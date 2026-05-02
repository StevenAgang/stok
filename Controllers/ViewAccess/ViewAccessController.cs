using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stok.Repository.Configurations.Helper;

namespace stok.Controllers.ViewAccess
{
    [ApiController]
    public class ViewAccessController(ResponseHelper response) : ControllerBase
    {
        private readonly ResponseHelper _response = response;

        [Authorize]
       public IActionResult Authorize()
        {

        }
    }
}
