using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Extensions;
using VakilPors.Data.Seeder;

namespace VakilPors.Data.Context;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    static AppDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<User>().HasIndex(user => user.UserName).IsUnique();
        modelBuilder.Entity<User>()
            .HasOne(u => u.Lawyer)
            .WithOne(l => l.User)
            .HasForeignKey<Lawyer>(l => l.UserId);

        modelBuilder.Entity<UserCommentLike>()
            .HasOne<User>(x => x.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserCommentLike>()
            .HasOne<ThreadComment>(x => x.Comment)
            .WithMany(c => c.UserLikes)
            .HasForeignKey(x => x.CommentId);

        modelBuilder.Entity<UserThreadLike>()
            .HasOne<User>(x => x.User)
            .WithMany(u => u.ThreadLikes)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserThreadLike>()
            .HasOne<ForumThread>(x => x.Thread)
            .WithMany(t => t.UserLikes)
            .HasForeignKey(x => x.ThreadId);


        modelBuilder.Entity<DocumentAccess>()
            .HasOne(x => x.Lawyer)
            .WithMany(l => l.DocumentAccesses)
            .HasForeignKey(x => x.LawyerId);

        modelBuilder.Entity<DocumentAccess>()
            .HasOne(x => x.Document)
            .WithMany(d => d.Accesses)
            .HasForeignKey(x => x.DocumentId);

        modelBuilder.Entity<Visitor>()
            .HasIndex(v => v.UserGUID)
            .IsUnique();

        base.OnModelCreating(modelBuilder);

        modelBuilder.RegisterEntities(typeof(User).Assembly);
        modelBuilder.ApplyUtcDateTimeConverter();
        DatabaseSeeder seeder = new DatabaseSeeder(modelBuilder);
        seeder.Seed();
    }

}

