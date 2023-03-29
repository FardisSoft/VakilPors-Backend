using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Context;

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
                bool hasAdmin=await db.Roles.AnyAsync(r=>r.Name==RoleNames.Admin);
                if (!hasAdmin){
                    //add default admin
                    using var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                    var admin=new User(){
                        Email="admin@fardissoft.ir",
                        Name="Admin",
                        PhoneNumber="09116863556",
                        UserName="09116863556"
                    };
                    await userManager.CreateAsync(admin,"admin");
                    await userManager.AddToRoleAsync(admin,RoleNames.Admin);
                }
            }
            catch (System.Exception)
            {
                Console.WriteLine("An error occurred while migrating or seeding the database.");
            }
        }
    }
}