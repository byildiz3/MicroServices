using CatologService.Api.Core.Domain;
using CatologService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Xml;

namespace CatologService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand",CatalogContext.DEFAULT_SCHEMA);
            builder.HasKey(xi=>xi.Id);
            builder.Property(ci => ci.Id)
                .UseHiLo("catalog_brand_hilo")
                .IsRequired();
            builder.Property(ci => ci.Brand)
                .IsRequired().HasMaxLength(100);



        }
    }
}
