using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFood.Domain.ValueObjects;

namespace TechFood.Infra.Persistence.ValueObjectMappings;

public static class PhoneMap
{
    public static void MapPhone<TEntity>(this OwnedNavigationBuilder<TEntity, Phone> navigationBuilder)
       where TEntity : class
    {
        navigationBuilder.WithOwner();

        navigationBuilder.Property(x => x.Number)
            .HasMaxLength(15)
            .HasColumnName("PhoneNumber")
            .IsRequired();

        navigationBuilder.Property(x => x.DDD)
            .HasMaxLength(4)
            .HasColumnName("PhoneDDD")
            .IsRequired();

        navigationBuilder.Property(x => x.CountryCode)
            .HasMaxLength(5)
            .HasColumnName("PhoneCountryCode")
            .IsRequired();
    }
}
