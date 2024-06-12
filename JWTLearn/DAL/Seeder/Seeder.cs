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
        _context.Garages.RemoveRange(_context.Garages);
        _context.Voitures.RemoveRange(_context.Voitures);

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

        if (!_context.Garages.Any())
        {
            _context.Garages.AddRange(Fixture.Garages);
        }

        if (!_context.Voitures.Any())
        {
            _context.Voitures.AddRange(Fixture.Voitures);
        }

        await _context.SaveChangesAsync();
    }
}
