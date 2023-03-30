using Microsoft.Extensions.DependencyInjection;
using VakilPors.Shared.Extensions;

namespace VakilPors.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services.RegisterScopedDependencies(assembly);
        services.RegisterSingletonDependencies(assembly);
        services.RegisterTransientDependencies(assembly);
    }

}
