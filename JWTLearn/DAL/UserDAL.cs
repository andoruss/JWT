using Context;
using DAL.Contract;
using Etities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class UserDAL(AppDbContext context) : IUserDAL
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// va chercher dans la table user si un user correspond à l'email
    /// </summary>
    /// <param name="email">email de l'utilisateur</param>
    /// <returns></returns>
    public async Task<User?> Login(string email)
    {
        return await _context.Users.Where(u => u.Email ==  email).FirstOrDefaultAsync();
    }
}
