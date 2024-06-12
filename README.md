1 - Création de la CI:

à la racine du projet créer un répertoire nomé : 
    
    ".github/workflows"
    
dans ce répertoire ajouter le fichier : 
    
    "ci.yml"

une fois créé ajouter ce contenu :
    
    name: SonarCloud

    on:
      push:
        branches:
          - dev
          - main
      pull_request:
        branches:
          - dev
          - main

    jobs:
      sonarcloud:
        runs-on: ubuntu-latest
        #defini les différentes tâches à exécuter 
        steps:
          - name: Checkout repository
            uses: actions/checkout@v2
    
          - name: SonarCloud Scan
            env:
              SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
              SONAR_PROJECT_KEY: ${{ secrets.SONAR_PROJECT_KEY }}
            run: |
              npm install -g sonarqube-scanner
              sonar-scanner \
                -Dsonar.projectKey=${{ secrets.SONAR_PROJECT_KEY }} \
                -Dsonar.organization=andoruss \
                -Dsonar.sources=. \
                -Dsonar.host.url=https://sonarcloud.io \
                -Dsonar.login=${{ secrets.SONAR_TOKEN }}

Les variables "SONAR_TOKEN" et "SONAR_PROJECT_KEY" son défini dans l'onglet Settings -> Secret and variable -> Action.
Créer deux nouvelles varaibles de repository qui on le même nome que celle dans le fichier de la ci.
Pour "SONAR_TOKEN" est égal au token généré quand le projet est créé sur sonarcloud alors que "SONAR_PROJECT_KEY" est la clé que l'on retrouve sur le projet sonnarcloud dans l'onglet information en bas à gauhce. L'organisation correspond au nom de l'oraganisation dans lequel le projet sonarCloud se trouve.

Lorsque la ci est lancé on peut voir le résultat du scan sur le projet sonarCloud.

2- Création de la base de données et de la migration: 

Installer dans la couche context et dans l'api les packages nugets ->
    
    Microsoft.EntityFrameworkCore
    Microsoft.EntityFrameworkCore.Design
    Microsoft.EntityFrameworkCore.SqlServer
    Microsoft.EntityFrameworkCore.Tools

Ajouter un fichier context dans la couche context ->

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {    
        public DbSet<Garage> Garages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Voiture> Voitures { get; set; }
        public DbSet<User> Users { get; set; }
    }

Ajout de la connexion string pour se connecter à la base de données dans les fichier settings de l'api->
Il faut au préalable créer une base de données vide.

    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=LearnJWT;Trusted_Connection=True;"
    }

Migration des entités dans la base de données -> 
aller dans le gestionnair de package et sélectionner le projet par default qui contient le dbContext

ajouter une migration :

    add-migration InitialMigration
    
mettre la base à jour :

    update-database --verbose

3 - Remplissage de la base de fausses données:
Ajouter le nuget package dans la couche Entities -> 

    Bogus

exemple de génération de données ->

    using Bogus;
using Microsoft.AspNetCore.Identity;

namespace Etities.Fixtures;

public class Fixture
{
    private readonly IPasswordHasher<User> _passwordHasher;

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

        public void HashPasswords()
        {
            foreach (var user in Users)
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
            }
        }
    }

Insertion des données dans la base ->
créer un seeder dans la couche dal pour insérer les données 

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

Appelle de l'initialisation dans le program.cs ->

    // Exécuter le seeder lors du démarrage de l'application
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {   
            // Récupérer le seeder et l'initialiser
            var seeder = services.GetRequiredService<ISeederContract>();
            await seeder.Initialize();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

Ajout des authorisations sur les routes

Hachage des mot de passe

Création d'un JWT 

