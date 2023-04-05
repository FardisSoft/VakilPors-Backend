using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Authentication.Extensions
{
    public static class IdentityExtensions
    {
        public static IdentityBuilder RegisterIdentity<TContext>(this IServiceCollection services) where TContext:DbContext{
            return services.AddIdentity<User,Role>(options=>{
                options.SignIn.RequireConfirmedPhoneNumber=false;
                options.User.RequireUniqueEmail=true;
                options.Password.RequireDigit=true;
                options.Password.RequiredUniqueChars=0;
                options.Password.RequireLowercase=true;
                options.Password.RequireNonAlphanumeric=false;
                options.Password.RequireUppercase=true;
                options.Password.RequiredLength=6;
                })
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();
            
        }
    }
}