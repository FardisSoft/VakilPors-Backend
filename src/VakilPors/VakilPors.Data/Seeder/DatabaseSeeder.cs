using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Data.Seeder
{
    public class DatabaseSeeder
    {
        private readonly ModelBuilder modelBuilder;
        public DatabaseSeeder(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }
        public void Seed()
        {
            seedIdentity();
        }
        private void seedIdentity()
        {
            //add roles
            List<Role> roles=new List<Role>();
            var roleNames=RoleNames.GetAll();
            var ConcurrencyStamps=new string[]{
                "8bdeb514-9677-4ab5-bcca-b7e097575597",
                "c7b51225-e9aa-4d81-8d75-b72266a01fde",
                "4e0f7985-9dbd-4d0f-9ff0-4420ee8fee75"
            };
            for (int i = 0; i < roleNames.Length; i++)
            {
                var roleName=roleNames[i];
                roles.Add(new Role
                {
                    Id = i+1,
                    Name = roleName.ToString(),
                    NormalizedName = roleName.ToString().ToUpper(),
                    ConcurrencyStamp=ConcurrencyStamps[i]
                });
            }
            modelBuilder.Entity<Role>().HasData(roles);
            //TODO: add admin user
        }

    }
}