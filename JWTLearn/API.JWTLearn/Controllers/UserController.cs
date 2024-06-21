using Microsoft.AspNetCore.Mvc;

namespace API.JWTLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<String>> Test()
    {
        return Ok("oui");
    }
}
