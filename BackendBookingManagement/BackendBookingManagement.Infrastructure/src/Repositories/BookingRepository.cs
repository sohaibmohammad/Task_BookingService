using BackendBookingManagement.Domain.src.Entity;
using BackendBookingManagement.Infrastructure.src.Database;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BackendBookingManagement.Infrastructure.src.Exensions;
using BackendBookingManagement.Application.src.Common;
using BackendBookingManagement.Application.src.Interfaces.Repositories;
namespace BackendBookingManagement.Infrastructure.src.Repositories;

public class BookingRepository(AppDbContext context) : Repository<Booking, Guid>(context), IBookingRepository
{
	public async Task<PagedResult<Booking>> GetBookingByStatusAsync(BookingStatus status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	{
		var query = _context.Set<Booking>().Where(b => b.Status == status);

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(b => b.StartDateTime)
			.ApplyPaging(pageNumber, pageSize)
			.ToListAsync(cancellationToken);

		return new PagedResult<Booking>(items, totalCount, pageNumber, pageSize);
	}

	public async Task<PagedResult<Booking>> GetBookingsByUserIdAsync(
		string userId,
		BookingStatus? status = null, 
		bool trackChanges = false,
		int pageNumber = 1,
		int pageSize = 10,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Bookings
			.ApplyTracking(trackChanges) 
			.Where(b => b.UserId == userId);

 		if (status.HasValue)
		{
			query = query.Where(b => b.Status == status.Value);
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
				.OrderByDescending(b => b.StartDateTime)
				.ApplyPaging(pageNumber, pageSize)
				.ToListAsync(cancellationToken);

		return new PagedResult<Booking>(items, totalCount, pageNumber, pageSize);
	}
	public async Task<bool> IsResourceAvailableAsync(string resourceId, DateTime startDateTime, DateTime endDateTime, Guid? excludeBookingId = null, CancellationToken cancellationToken = default)
	{
		var query = _context.Bookings
			.AsNoTracking()
			.Where(b => b.ResourceId == resourceId &&
						b.Status == BookingStatus.Confirmed &&
						b.StartDateTime < endDateTime &&
						b.EndDateTime > startDateTime);

		if (excludeBookingId.HasValue)
		{
			query = query.Where(b => b.Id != excludeBookingId.Value);
		}

		return !await query.AnyAsync(cancellationToken);
	}

	public async Task<PagedResult<Booking>> GetBookingsByResourceIdAsync(
	string resourceId,
	int pageNumber,
	int pageSize,
	BookingStatus? status = null,
	bool trackChanges = false,
	CancellationToken cancellationToken = default)
	{
		var query = _context.Bookings
			.ApplyTracking(trackChanges)
			.Where(b => b.ResourceId == resourceId);

		if (status.HasValue)
		{
			query = query.Where(b => b.Status == status.Value);
		}
		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(b => b.StartDateTime)
			.ApplyPaging(pageNumber, pageSize)
			.ToListAsync(cancellationToken);

		return new PagedResult<Booking>(items, totalCount, pageNumber, pageSize);
	}
	public async Task<Booking?> GetByIdWithResourceAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default)
	{
		return await _context.Bookings
			.ApplyTracking(trackChanges)
			.Include(b => b.Resource)  
			.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
	}


	public async Task<IEnumerable<Booking>> GetActiveBookingsForResourceAsync(string resourceId, DateTime startOfDay, DateTime endOfDay)
	{
		return await _context.Bookings
			.Where(b => b.ResourceId == resourceId &&
						b.Status != BookingStatus.Canceled &&
						b.StartDateTime < endOfDay &&
						b.EndDateTime > startOfDay)
			.ToListAsync();
	}
}
