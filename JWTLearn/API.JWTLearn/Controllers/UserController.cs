using Etities.Request;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;

namespace API.JWTLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService,
    IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpPost]
    public async Task<ActionResult<String>> Test([FromBody] LoginRequest body)
    {
        if(body.Email == null || body.Password == null)
            return BadRequest("L'email ou le mot de passe n'est pas renseigné");

        var response = await _authenticationService.Login(body.Email, body.Password);

        return response != null ? Ok(response) : NotFound("User introuvable ou mot de passe incorrect");
    }
}
