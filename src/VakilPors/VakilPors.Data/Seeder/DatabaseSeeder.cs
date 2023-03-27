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
            for (int i = 0; i < roleNames.Length; i++)
            {
                var roleName=roleNames[i];
                roles.Add(new Role
                {
                    Id = i+1,
                    Name = roleName.ToString(),
                    NormalizedName = roleName.ToString().ToUpper(),
                });
            }
            modelBuilder.Entity<Role>().HasData(roles);
            //TODO: add admin user
        }

    }
}