using Context;
using DAL.Contract.Seeder;
using Etities;
using Etities.Fixtures;
using Microsoft.AspNetCore.Identity;

namespace DAL.Seeder;

public class Seeder(AppDbContext context) : ISeederContract
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Supprimer toutes les données des tables
    /// </summary>
    private async Task ClearDatabase()
    {
        _context.Roles.RemoveRange(_context.Roles);
        _context.Users.RemoveRange(_context.Users);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// ajout tous les objets dans la base de données
    /// </summary>
    public async Task Initialize()
    {
        await ClearDatabase();

        var fixture = new Fixture(new PasswordHasher<User>());

        if (!_context.Roles.Any())
        {
            _context.Roles.AddRange(Fixture.Roles);
        }

        if (!_context.Users.Any())
        {
            _context.Users.AddRange(Fixture.Users);
        }

        await _context.SaveChangesAsync();
    }
}
