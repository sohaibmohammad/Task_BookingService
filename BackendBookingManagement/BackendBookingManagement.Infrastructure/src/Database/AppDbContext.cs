using BackendBookingManagement.Domain.src.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Database;

public  class AppDbContext:DbContext
{
	public DbSet<Resource> Resources { get; set; }
	public DbSet<Booking> Bookings { get; set; }
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

}
