using Context;
using DAL;
using DAL.Contract;
using DAL.Contract.Seeder;
using DAL.Seeder;
using Etities;
using Etities.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;
using Service.Contract;
using System;

var builder = WebApplication.CreateBuilder(args);

//
// Set up configuration sources.
//

builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication:Local"));

// Add services to the container.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ISeederContract, Seeder>();
builder.Services.AddScoped<IUserDAL, UserDAL>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ajout de la connexion string.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


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


app.Run();
