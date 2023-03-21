using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VakilPors.Shared.Entities;

namespace VakilPors.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void RegisterEntities(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(IEntity).IsAssignableFrom(c));

        foreach (Type type in types)
            modelBuilder.Entity(type);
    }
}
