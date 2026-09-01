using BackendBookingManagement.Domain.src.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Configuration
{
	public class BookingConfiguration : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
		{
			builder.ToTable("Bookings");

			builder.HasKey(b => b.Id);

			builder.Property(b => b.ResourceId)
				  .IsRequired()
				  .HasMaxLength(50);

			builder.Property(b => b.UserId)
				  .IsRequired()
				  .HasMaxLength(50);

			builder.Property(b => b.StartDateTime)
				  .IsRequired();

			builder.Property(b => b.EndDateTime)
				  .IsRequired();

			builder.Property(b => b.Status)
				  .IsRequired();

			builder.Property(b => b.CreatedAt)
				  .IsRequired();

			builder.HasIndex(b => new { b.ResourceId, b.Status, b.StartDateTime });

			builder.HasOne(b => b.Resource)
				  .WithMany(r => r.Bookings)
				  .HasForeignKey(b => b.ResourceId)
				  .OnDelete(DeleteBehavior.Cascade);
		}
	}
	public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
	{
		public void Configure(EntityTypeBuilder<Resource> builder)
		{
			builder.ToTable("Resources");

			builder.HasKey(r => r.Id);

 			builder.Property(r => r.Id)
				  .HasMaxLength(50);

			builder.Property(r => r.Name)
				  .IsRequired()
				  .HasMaxLength(100);

			builder.Property(r => r.Description)
				  .HasMaxLength(500);
		}
	}
}
