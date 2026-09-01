using BackendBookingManagement.Application.Interfaces;
using BackendBookingManagement.Infrastructure.src.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Repositories
{
	internal class UnitOfWork(AppDbContext context ,IBookingRepository bookingRepository) : IUnitOfWork
	{
		private readonly AppDbContext _context = context;
		public IBookingRepository Bookings { get; }= bookingRepository;

		public Task<int> CompleteAsync(CancellationToken cancellationToken = default)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
