using Etities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
}
