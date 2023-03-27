using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Extensions;
using VakilPors.Data.Seeder;

namespace VakilPors.Data.Context;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(user => user.UserName).IsUnique();

        modelBuilder.RegisterEntities(typeof(User).Assembly);
        DatabaseSeeder seeder=new DatabaseSeeder(modelBuilder);
        seeder.Seed();
    }
    
}

