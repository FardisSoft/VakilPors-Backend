using System.Reflection;
using VakilPors.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace VakilPors.Shared.Extensions;

public static class DIExtensions
{
    public static void RegisterScopedDependencies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (Assembly assembly in assemblies)
        {
            GetConcreteTypes<IScopedDependency>(assembly).ForEach(type =>
            {
                Type typeInterface = GetTypeInterface(type);
                if (typeInterface != null)
                    services.AddScoped(typeInterface, type);
                else
                    services.AddScoped(type);
            });
        }
    }

    public static void RegisterTransientDependencies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (Assembly assembly in assemblies)
        {
            GetConcreteTypes<ITransientDependency>(assembly).ForEach(type =>
            {
                Type typeInterface = GetTypeInterface(type);
                if (typeInterface != null)
                    services.AddTransient(typeInterface, type);
                else
                    services.AddTransient(type);
            });
        }
    }

    public static void RegisterSingletonDependencies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (Assembly assembly in assemblies)
        {
            GetConcreteTypes<ISingletonDependency>(assembly).ForEach(type =>
            {
                Type typeInterface = GetTypeInterface(type);
                if (typeInterface != null)
                    services.AddSingleton(typeInterface, type);
                else
                    services.AddSingleton(type);
            });
        }
    }


    private static List<Type> GetConcreteTypes<TIDependencyType>(Assembly assembly)
        where TIDependencyType : IDependencyType
        => assembly.GetTypes().Where(x =>
            x.IsClass &&
            x.IsPublic &&
            !x.IsAbstract &&
            typeof(TIDependencyType).IsAssignableFrom(x)).ToList();

    private static Type GetTypeInterface(Type concreteType)
        => concreteType.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name.Contains(concreteType.Name));

}

