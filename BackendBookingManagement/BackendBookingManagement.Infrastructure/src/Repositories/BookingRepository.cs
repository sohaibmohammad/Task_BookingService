using BackendBookingManagement.Application.Common;
using BackendBookingManagement.Application.Interfaces;
using BackendBookingManagement.Domain.src.Entity;
using BackendBookingManagement.Infrastructure.src.Database;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BackendBookingManagement.Infrastructure.src.Exensions;
namespace BackendBookingManagement.Infrastructure.src.Repositories;

public class BookingRepository(AppDbContext context) : Repository<Booking, Guid>(context), IBookingRepository
{
	public async Task<PagedResult<Booking>> GetBookingByStatusAsync(string status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	{
		var query = _context.Set<Booking>().Where(b => b.Status.ToString() == status);

		var totalCount = await query.CountAsync(cancellationToken);

		var items =await query.ApplyPaging(pageNumber, pageSize)
			.ToListAsync(cancellationToken);
		return new PagedResult<Booking>(items, totalCount, pageNumber, pageSize);
	}

	public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		var bookings = await _context.Bookings.ApplyTracking(false)
			.Where(b => b.UserId == userId)
			.ToListAsync(cancellationToken);
		return bookings;
	}
}
