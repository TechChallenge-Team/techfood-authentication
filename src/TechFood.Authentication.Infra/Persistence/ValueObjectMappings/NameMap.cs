using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFood.Authentication.Domain.ValueObjects;

namespace TechFood.Authentication.Infra.Persistence.ValueObjectMappings;

public static class NameMap
{
    public static void MapName<TEntity>(this OwnedNavigationBuilder<TEntity, Name> navigationBuilder)
        where TEntity : class
    {
        navigationBuilder.WithOwner();

        navigationBuilder.Property(x => x.FullName)
            .HasMaxLength(255)
            .HasColumnName("NameFullName")
            .IsRequired();
    }
}
