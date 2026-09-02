using BackendBookingManagement.Application.src.Interfaces.Repositories;
using BackendBookingManagement.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Repositories
{
	public class UnitOfWork(AppDbContext context ,IBookingRepository bookingRepository) : IUnitOfWork
	{
		private readonly AppDbContext _context = context;
		public IBookingRepository Bookings { get; }= bookingRepository;

		public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
		}

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
