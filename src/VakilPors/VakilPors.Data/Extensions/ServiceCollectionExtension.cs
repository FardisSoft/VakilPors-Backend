using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Data.Context;
using VakilPors.Data.Repositories;
using VakilPors.Data.UnitOfWork;

namespace VakilPors.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterAppDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
        {
            options.UseNpgsql(connectionString);
            options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });
        services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
        services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
    }
}

