using Application.Features.Identity.Auth.Queries.Login;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1.Identity;

[Route("Api/V{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
public class AuthController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<ResponseContainer<LoginQueryResponse>>> LoginAsync(LoginQuery query)
    {
        ResponseContainer<LoginQueryResponse> response = await Mediator.Send(query);

        if (!response.IsSucceed)
        {
            return Unauthorized(response.ErrorMessage);
        }

        return Ok(response);
    }
}
