using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Data.Context;
using VakilPors.Data.Repositories;
using VakilPors.Data.UnitOfWork;

namespace VakilPors.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("AppDbContext"));
        });
        services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
        services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
    }
}

