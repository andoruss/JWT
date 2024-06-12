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
        new() { Id = Guid.NewGuid(), Libelle = "Garagiste" },
        new() { Id = Guid.NewGuid(), Libelle = "Acheteur" }
    ];

    public static List<User> Users { get; set; } = [];
        
    public static List<Garage> Garages { get; set; } = [];

    public static List<Voiture> Voitures { get; set; } = [];

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

        Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = "beney",
            Email = "beney",
            Password = "beney",
            Role = Roles[2]
        });

        var voitureGenerate = new Faker<Voiture>()
            .RuleFor(v => v.Id, f => Guid.NewGuid())
            .RuleFor(v => v.Marque, f => f.Vehicle.Manufacturer())
            .RuleFor(v => v.Libelle, f => f.Vehicle.Model())
            .RuleFor(v => v.Immatriculation, f => f.Vehicle.Vin())
            .RuleFor(v => v.Prix, f => f.Random.Float(1000, 100000));
        Voitures = voitureGenerate.Generate(2000);

        var garageGenerate = new Faker<Garage>()
            .RuleFor(g => g.Id, f => Guid.NewGuid())
            .RuleFor(g => g.Name, f => f.Company.CompanyName())
            .RuleFor(g => g.Address, f => f.Address.FullAddress())
            .RuleFor(g => g.Voitures, f => f.PickRandom(Voitures, 10).ToList());
        Garages = garageGenerate.Generate(100);

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
