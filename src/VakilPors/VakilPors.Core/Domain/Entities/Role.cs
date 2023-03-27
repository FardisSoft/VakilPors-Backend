using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VakilPors.Core.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        
    }
    public static class RoleNames
    {
        public const string User = "User";
        public const string Vakil = "Vakil";
        public const string Admin = "Admin";
        public static string[] GetAll()
        {
            return new string[] {User, Vakil, Admin };
        }
    }
}