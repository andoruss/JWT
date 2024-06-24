using Etities;

namespace DAL.Contract;

public interface IUserDAL
{
    Task<User?> Login(string email);
}
