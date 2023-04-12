﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Extensions;
using VakilPors.Data.Seeder;

namespace VakilPors.Data.Context;

public class AppDbContext : IdentityDbContext<User, Role, int>
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>().HasIndex(user => user.UserName).IsUnique();
        modelBuilder.Entity<User>()
            .HasOne(u=>u.Lawyer)
            .WithOne(l=>l.User)
            .HasForeignKey<Lawyer>(l=>l.UserId);
        modelBuilder.RegisterEntities(typeof(User).Assembly);
        modelBuilder.ApplyUtcDateTimeConverter();
        DatabaseSeeder seeder = new DatabaseSeeder(modelBuilder);
        seeder.Seed();
    }

}

