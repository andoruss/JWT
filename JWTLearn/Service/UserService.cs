using DAL.Contract;
using Etities;
using Microsoft.AspNetCore.Identity;
using Service.Contract;

namespace Service;

public class UserService(IUserDAL dal, IPasswordHasher<User> passwordHasher) : IUserService
{
    private readonly IUserDAL _dal = dal;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

}
