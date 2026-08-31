using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BackendBookingManagement.Infrastructure.src.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

		optionsBuilder.UseSqlServer("Server=localhost,1433;Database=BookingManagementDb;User Id=sa;Password=Sohaib@12;TrustServerCertificate=True;");

		return new AppDbContext(optionsBuilder.Options);
	}
}
