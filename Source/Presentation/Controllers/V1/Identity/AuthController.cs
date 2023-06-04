using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1.Identity;

[Route("Api/V{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
public class AuthController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
