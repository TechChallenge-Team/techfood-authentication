using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFood.Domain.ValueObjects;

namespace TechFood.Infra.Persistence.ValueObjectMappings;

public static class EmailMap
{
    public static void MapEmail<TEntity>(this OwnedNavigationBuilder<TEntity, Email> navigationBuilder)
    where TEntity : class
    {
        navigationBuilder.WithOwner();

        navigationBuilder.Property(x => x.Address)
            .HasMaxLength(255)
            .HasColumnName("EmailAddress")
            .IsRequired();
    }
}
