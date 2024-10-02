using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            // Configure the FuelType to be stored as string
            builder.Property(c => c.Fuel)
                   .HasConversion<string>();

            // Configure the TransmissionType to be stored as string
            builder.Property(c => c.Transmission)
                   .HasConversion<string>();

            // Configure the CarCondition to be stored as string
            builder.Property(c => c.CarCondition)
                   .HasConversion<string>();

            // Configure the ModelVariant to be stored as string
            builder.Property(c => c.ModelVariant)
                   .HasConversion<string>();

            builder.Property(e => e.Price)
            .HasColumnType("decimal(18, 2)");
        }
    }
}
