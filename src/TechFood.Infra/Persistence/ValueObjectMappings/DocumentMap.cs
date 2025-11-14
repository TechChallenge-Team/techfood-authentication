using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFood.Domain.ValueObjects;

namespace TechFood.Infra.Persistence.ValueObjectMappings;

public static class DocumentMap
{
    public static void MapDocument<TEntity>(this OwnedNavigationBuilder<TEntity, Document> navigationBuilder)
       where TEntity : class
    {
        navigationBuilder.WithOwner();

        navigationBuilder.Property(x => x.Value)
            .HasMaxLength(20)
            .HasColumnName("DocumentValue")
            .IsRequired();

        navigationBuilder.Property(x => x.Type)
            .HasColumnName("DocumentType")
            .IsRequired();
    }
}
