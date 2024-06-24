using Etities.Configuration;
using Service.Contract;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
//permet de créer une instance de jwt
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Etities;
using Microsoft.AspNetCore.Identity;
using DAL.Contract;

namespace Service;

public class AuthenticationService(IOptions<AuthenticationSettings> authenticationSettings,
    IUserDAL dal,
    IPasswordHasher<User> passwordHasher) : IAuthenticationService
{
    private readonly AuthenticationSettings _authenticationSettings = authenticationSettings.Value;
    private readonly IUserDAL _dal = dal;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;


    /// <summary>
    /// permet de récupérer de vérifier si le mot de passe entré correspond bien au mot de passe de l'email saisi
    /// </summary>
    /// <param name="email">email de l'utilisateur</param>
    /// <param name="password">password non haché</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string?> Login(string email, string password)
    {
        var response = await _dal.Login(email);

        if (response == null)
            return null;
        //verifie si le password et le même que celui haché dans la base
        var result = _passwordHasher.VerifyHashedPassword(response, response.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return this.GetJWT(response);
    }

    /// <summary>
    /// permet de générer le JWT token
    /// </summary>
    /// <param name="user">user qui va transmettre les infos dans le jwt</param>
    /// <returns></returns>
    private string GetJWT(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        //récupère la clé secret dans le fichier appsettings
        var key = Encoding.ASCII.GetBytes(_authenticationSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Surname, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            }),
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_authenticationSettings.ExpiryMinutes)),
            //récupère l'audience dans le fichier appsettings
            Audience = _authenticationSettings.Audience,
            //récupère l'issuer dans le fichier appsettings
            Issuer = _authenticationSettings.Issuer,
            //signe le jwt
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
