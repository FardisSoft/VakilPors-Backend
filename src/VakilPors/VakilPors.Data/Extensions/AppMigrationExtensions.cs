using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Context;
using VakilPors.Data.Seeder;

namespace VakilPors.Data.Extensions
{
    public static class AppMigrationExtensions
    {
        public static async Task MigrateDb(this WebApplication app)
        {
            try
            {
                await using var scope = app.Services.CreateAsyncScope();
                using var db = scope.ServiceProvider.GetService<AppDbContext>();
                await db.Database.MigrateAsync();
                Console.WriteLine("Migrating database...");
                int adminRoleId = await db.Roles.Where(r => r.Name == RoleNames.Admin).Select(r => r.Id).FirstOrDefaultAsync();
                bool hasAdmin = await db.UserRoles.AnyAsync(r => r.RoleId == adminRoleId);
                if (!hasAdmin)
                {
                    using var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                    //add default admin
                    var admin = new User()
                    {
                        Email = "info@mail.fardissoft.ir",
                        Name = "Admin",
                        PhoneNumber = "09116863556",
                        UserName = "09116863556"
                    };
                    await userManager.CreateAsync(admin, "Admin123");
                    await userManager.AddToRoleAsync(admin, RoleNames.Admin);
                    // await userManager.AddToRoleAsync(admin, RoleNames.User);
                    Console.WriteLine("Creating default admin user...");
                }
                //add other users
                int numUsers = await db.Users.CountAsync();
                if (numUsers < DatabaseSeeder.countUsers)
                {
                    // var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                    await DatabaseSeeder.seedUsersAndLawyers(db);
                    // userManager.Dispose();
                }
                int numTrans = await db.Set<Transaction>().CountAsync();
                if (numTrans < DatabaseSeeder.countTrans)
                {
                    var trans = DatabaseSeeder.seedTransactions().ToArray();
                    await db.AddRangeAsync(trans);
                    await db.SaveChangesAsync();
                    var walletService = scope.ServiceProvider.GetService<IWalletServices>();
                    await applyTransactions(walletService, trans);
                }
            }
            catch (System.Exception)
            {
                Console.WriteLine("An error occurred while migrating or seeding the database.");
            }
        }
        private static async Task applyTransactions(IWalletServices walletServices, Transaction[] tranactions)
        {
            foreach (var trans in tranactions)
            {
                await walletServices.ApplyTransaction(trans.Id);
            }
        }
    }
}