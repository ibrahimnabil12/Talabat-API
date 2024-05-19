using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Configuratons
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p=>p.PictureUrl)
                .IsRequired();

            builder.Property(p=>p.Price)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(p => p.ProductBrand)
                .WithMany()
                .HasForeignKey(p => p.ProductBrandId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(P=>P.ProductBrandId).IsRequired(false);
            builder.Property(P => P.ProductTypeId).IsRequired(false);

        }
    }
}
