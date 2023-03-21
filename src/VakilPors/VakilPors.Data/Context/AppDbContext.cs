using Microsoft.EntityFrameworkCore;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Extensions;

namespace VakilPors.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();

        modelBuilder.RegisterEntities(typeof(User).Assembly);
    }
}

