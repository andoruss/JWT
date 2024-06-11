using Microsoft.AspNetCore.Mvc;

namespace API.JWTLearn.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public bool Test()
    {
        return true;
    }
}
