using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Context;
using VakilPors.Shared.Services;

namespace VakilPors.Data.Seeder
{
    public class DatabaseSeeder
    {
        private readonly ModelBuilder modelBuilder;
        const int seed = 100;
        public DatabaseSeeder(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
            Randomizer.Seed = new Random(seed);
        }
        public void Seed()
        {
            seedPlans();
            seedIdentity();
        }

        private void seedPlans()
        {
            const int seed = 4;
            List<Premium> plans = new List<Premium>();
            for (int i = 0; i < seed; i++)
            {
                plans.Add(new Premium() { Id = i + 1, ServiceType = (Plan)i });
            }
            modelBuilder.Entity<Premium>().HasData(plans);
        }
        private void seedIdentity()
        {
            //add roles
            List<Role> roles = new List<Role>();
            var roleNames = RoleNames.GetAll();
            var ConcurrencyStamps = new string[]{
                "8bdeb514-9677-4ab5-bcca-b7e097575597",
                "c7b51225-e9aa-4d81-8d75-b72266a01fde",
                "4e0f7985-9dbd-4d0f-9ff0-4420ee8fee75"
            };
            for (int i = 0; i < roleNames.Length; i++)
            {
                var roleName = roleNames[i];
                roles.Add(new Role
                {
                    Id = i + 1,
                    Name = roleName.ToString(),
                    NormalizedName = roleName.ToString().ToUpper(),
                    ConcurrencyStamp = ConcurrencyStamps[i]
                });
            }
            modelBuilder.Entity<Role>().HasData(roles);
        }
        public const int countUsers = 400;
        public const int countTrans = 200;
        public const int startUserId = 100;
        public static IEnumerable<Transaction> seedTransactions()
        {
            Randomizer.Seed = new Random(seed);
            var fakerTrans = new Faker<Transaction>()
            .RuleFor(t => t.Amount, f => f.Random.Decimal() * f.Random.Int(1000, 1000_000))
            .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
            .RuleFor(t => t.IsIncome, f => f.Random.Bool())
            .RuleFor(t => t.IsSuccess, f => f.Random.Bool())
            .RuleFor(t => t.UserId, f => f.Random.Int(startUserId, startUserId + countUsers - 1));

            var fakeData = fakerTrans.GenerateLazy(countTrans);
            return fakeData;
            // modelBuilder.Entity<Role>().HasData(roles);
        }
        public static async Task seedUsersAndLawyers(AppDbContext context)
        {
            Randomizer.Seed = new Random(seed);
            int uid = startUserId;
            int countUserslocal = countUsers / 3;
            // long phoneNumber=09116863557;
            const int startSubscribedId = 1;
            int subscribedId = startSubscribedId;
            var fakerUser = new Faker<User>("fa")
            .RuleFor(u => u.Id, f => uid++)
            .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString())
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone)
            .RuleFor(u => u.UserName, (f, u) => u.PhoneNumber)
            .RuleFor(u => u.LawyerId, f => 0);
            var fakerSubscribed = new Faker<Subscribed>()
                .RuleFor(u => u.ID, f => subscribedId++)
                .RuleFor(u => u.UserId, f => uid++)
                .RuleFor(u => u.PremiumID, f => 1);

            var users = fakerUser.Generate(countUserslocal);
            subscribedId = startSubscribedId;
            uid = startUserId;
            var subscribes = fakerSubscribed.Generate(countUserslocal);
            const string password = "Password123";
            int userRoleId = RoleNames.GetAll().ToList().IndexOf(RoleNames.User) + 1;
            int vakilRoleId = RoleNames.GetAll().ToList().IndexOf(RoleNames.Vakil) + 1;
            // var ids=new List<int>();
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            for (int i = 0; i < users.Count; i++)
            {
                users[i].PasswordHash = hasher.HashPassword(users[i], password);
                // modelBuilder.Entity<User>().HasData(users[i]);
                // modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>(){
                //     UserId=users[i].Id,
                //     RoleId=userRoleId
                // });
                await context.Users.AddAsync(users[i]);
                await context.Set<Subscribed>().AddAsync(subscribes[i]);
                await context.SaveChangesAsync();
                await context.UserRoles.AddAsync(new IdentityUserRole<int>()
                {
                    RoleId = userRoleId,
                    UserId = users[i].Id
                });
                await context.SaveChangesAsync();
                // userManager.CreateAsync(users[i], password).Wait();
                // users[i]=await context.Set<User>().FirstOrDefaultAsync(u=>u.UserName==users[i].UserName);
                // userManager.AddToRoleAsync(users[i], RoleNames.Vakil).Wait();
            }
            var ids = users.Select(u => u.Id).ToList();
            users = fakerUser.Generate(2 * countUserslocal);
            for (int i = 0; i < users.Count; i++)
            {
                users[i].PasswordHash = hasher.HashPassword(users[i], password);
                // modelBuilder.Entity<User>().HasData(users[i]);
                // modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>(){
                //     UserId=users[i].Id,
                //     RoleId=vakilRoleId
                // });
                await context.Users.AddAsync(users[i]);
                await context.SaveChangesAsync();
                await context.UserRoles.AddAsync(new IdentityUserRole<int>()
                {
                    RoleId = vakilRoleId,
                    UserId = users[i].Id
                });
                await context.SaveChangesAsync();
                // userManager.CreateAsync(users[i], password).Wait();
                // await context.SaveChangesAsync();
                // users[i]=await context.Set<User>().FirstOrDefaultAsync(u=>u.UserName==users[i].UserName);
                // userManager.AddToRoleAsync(users[i], RoleNames.User).Wait();
            }
            int vid_index = 0;
            // int vid = 1001;
            var types =new [] {"جنایی","حقوقی","انتخابی","معاضدتی","سازمانی","تسخیری" };
            var fakerLawyer = new Faker<Lawyer>()
                .RuleFor(l => l.UserId, f => ids[vid_index++])
                .RuleFor(l => l.LicenseNumber, f => f.Random.Int(10000, 99999).ToString())
                .RuleFor(l => l.City, f => f.Address.City())
                .RuleFor(l => l.Title, f => f.PickRandom(types))
                .RuleFor(l => l.AboutMe, f => f.Lorem.Paragraph())
                .RuleFor(l => l.YearsOfExperience, f => f.Random.Int(1, 10))
                .RuleFor(l => l.OfficeAddress, f => f.Address.FullAddress())
                .RuleFor(l => l.Specialties, f => f.Lorem.Sentence())
                .RuleFor(l => l.MemberOf, f => f.Lorem.Word())
                .RuleFor(l => l.ProfileImageUrl, (f, l) => $"https://i.pravatar.cc/150?u={l.Title}");

            var lawyers = fakerLawyer.Generate(countUserslocal);
            await context.Set<Lawyer>().AddRangeAsync(lawyers);
            await context.SaveChangesAsync();
            foreach (var lawyer in lawyers)
            {
                var user = await context.Users.FindAsync(lawyer.UserId);
                user.LawyerId = lawyer.Id;
                context.Update(user);
            }
            await context.SaveChangesAsync();
            // userManager.Dispose();
            // modelBuilder.Entity<Lawyer>().HasData(fakerLawyer.Generate(countUsers));
        }
    }
}