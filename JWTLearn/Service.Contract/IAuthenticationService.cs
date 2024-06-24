using Etities;

namespace Service.Contract;

public interface IAuthenticationService
{
    Task<string> Login(string email, string password);
}
