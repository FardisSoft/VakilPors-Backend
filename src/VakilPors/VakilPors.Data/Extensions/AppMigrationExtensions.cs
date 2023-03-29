using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            }
            catch (System.Exception)
            {
                Console.WriteLine("An error occurred while migrating or seeding the database.");
            }
        }
    }
}