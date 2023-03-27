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
                options.SignIn.RequireConfirmedPhoneNumber=true;
                options.User.RequireUniqueEmail=true;
                options.Password.RequireDigit=true;
                options.Password.RequiredUniqueChars=1;
                options.Password.RequireLowercase=true;
                options.Password.RequireNonAlphanumeric=false;
                options.Password.RequireUppercase=true;
                })
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();
            
        }
    }
}