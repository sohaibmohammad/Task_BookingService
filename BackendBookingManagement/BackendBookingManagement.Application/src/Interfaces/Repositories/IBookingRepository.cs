using BackendBookingManagement.Application.src.Common;
using BackendBookingManagement.Domain.src.Entity;

namespace BackendBookingManagement.Application.src.Interfaces.Repositories;

public interface IBookingRepository : IRepository<Booking, Guid>
{
	public Task<PagedResult<Booking>> GetBookingByStatusAsync(BookingStatus status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
	Task<bool> IsResourceAvailableAsync(string resourceId, DateTime startDateTime, DateTime endDateTime, Guid? excludeBookingId = null, CancellationToken cancellationToken = default);
 	Task<PagedResult<Booking>> GetBookingsByResourceIdAsync(string resourceId, int pageNumber, int pageSize, BookingStatus? status = null, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<Booking?> GetByIdWithResourceAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);
	Task<PagedResult<Booking>> GetBookingsByUserIdAsync(string userId, BookingStatus? status = null, bool trackChanges = false, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
	Task<IEnumerable<Booking>> GetActiveBookingsForResourceAsync(string resourceId, DateTime startOfDay, DateTime endOfDay);
	Task<PagedResult<Booking>> GetBookingsByResourceWithDateRangeAsync(string resourceId, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 10, BookingStatus? status = null, bool trackChanges = false, CancellationToken cancellationToken = default);
}