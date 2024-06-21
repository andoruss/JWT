using Bogus;
using Microsoft.AspNetCore.Identity;

namespace Etities.Fixtures;

public class Fixture
{
    private readonly IPasswordHasher<User> _passwordHasher;

    /// <summary>
    /// IPasswordHasher<User> est injecté pour hacher les mots de passes
    /// </summary>
    /// <param name="passwordHasher"></param>
    public Fixture(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
        GenerateFixtures();
        HashPasswords();
    }

    public static List<Role> Roles { get; set; } =
    [
        new() { Id = Guid.NewGuid(), Libelle = "Admin" },
        new() { Id = Guid.NewGuid(), Libelle = "User" }
    ];

    public static List<User> Users { get; set; } = [];

    /// <summary>
    /// génère les données de test
    /// </summary>
    public void GenerateFixtures()
    {
        var userGenerator = new Faker<User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.Role, f => f.PickRandom(Roles))
            .RuleFor(u => u.CreatedAt, DateTime.Now);
        Users = userGenerator.Generate(29);

        Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = "antoine",
            Email = "antoine",
            Password = "antoine",
            Role = Roles[0]
        });

        Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = "aurelien",
            Email = "aurelien",
            Password = "aurelien",
            Role = Roles[1]
        });
    }

    /// <summary>
    /// hache les mots de passes de chaque utilisateur
    /// </summary>
    public void HashPasswords()
    {
        foreach (var user in Users)
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
        }
    }
}
