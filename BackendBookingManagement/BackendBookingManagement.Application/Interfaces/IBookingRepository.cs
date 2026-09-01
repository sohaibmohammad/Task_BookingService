using BackendBookingManagement.Application.Common;
using BackendBookingManagement.Domain.src.Entity;

namespace BackendBookingManagement.Application.Interfaces;

public interface IBookingRepository : IRepository<Booking, Guid>
{
	public  Task<PagedResult<Booking>> GetBookingByStatusAsync(BookingStatus status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	Task<bool> IsResourceAvailableAsync(string resourceId, DateTime startDateTime, DateTime endDateTime, Guid? excludeBookingId = null, CancellationToken cancellationToken = default);
	Task<PagedResult<Booking>> GetBookingsByUserIdAsync(string userId, bool trackChanges = false, int pageNumber = 0, int pageSize = 0, CancellationToken cancellationToken = default);
	Task<PagedResult<Booking>> GetBookingsByResourceIdAsync(string resourceId, int pageNumber, int pageSize, BookingStatus? status = null, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<Booking?> GetByIdWithResourceAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);
}