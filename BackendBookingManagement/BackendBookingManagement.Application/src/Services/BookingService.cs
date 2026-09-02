using BackendBookingManagement.Application.src.Common;
using BackendBookingManagement.Application.src.DTOs.Bookings;
using BackendBookingManagement.Application.src.Interfaces.Repositories;
using BackendBookingManagement.Application.src.Interfaces.Services;
using BackendBookingManagement.Application.src.Mappings;
using BackendBookingManagement.Domain.src.Entity;

namespace BackendBookingManagement.Application.src.Services;


public class BookingService(IBookingRepository bookingRepository, IUnitOfWork unitOfWork) : IBookingService
{
	public async Task CancelBookingAsync(Guid id)
	{
		var booking = await bookingRepository.GetByIdAsync(id);
		

		if (booking == null)
		{
			throw new KeyNotFoundException("The reservation is not available..");
		}

		if (booking.Status == BookingStatus.Canceled)
		{
			throw new InvalidOperationException("The reservation is not available.");
		}
		if (booking.StartDateTime <= DateTime.UtcNow)
		{
			throw new InvalidOperationException("You cannot cancel a booking that has already started or passed.");
		}

		booking.Status = BookingStatus.Canceled;

		bookingRepository.Update(booking);
		await unitOfWork.CompleteAsync();
	}

	public async Task<IEnumerable<BookedTimeSlotDto>> GetBookedTimeSlotsAsync(string resourceId, DateTime date)
	{
 		var startOfDay = date.Date;
		var endOfDay = date.Date.AddDays(1).AddTicks(-1);
		var bookings = await unitOfWork.Bookings.GetActiveBookingsForResourceAsync(resourceId, startOfDay, endOfDay);

 		return bookings.Select(b => new BookedTimeSlotDto
		{
			StartDateTime = b.StartDateTime,
			EndDateTime = b.EndDateTime
		}).OrderBy(b => b.StartDateTime).ToList();
	}

	public async Task<BookingResponseDto?> CreateBookingAsync(CreateBookingDto dto)
	{
		using var transaction = await unitOfWork.BeginTransactionAsync();
		try
		{
			var isAvailable = await bookingRepository.IsResourceAvailableAsync(
				dto.ResourceId,
				dto.StartDateTime,
				dto.EndDateTime
			);

			if (!isAvailable)
			{
				throw new InvalidOperationException("The resource is unavailable at this specific time.");
			}

			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				ResourceId = dto.ResourceId,
				UserId = dto.UserId,
				StartDateTime = dto.StartDateTime,
				EndDateTime = dto.EndDateTime,
				CreatedAt = DateTime.UtcNow,
				Status = BookingStatus.Confirmed
			};

			await bookingRepository.AddAsync(booking);
			await unitOfWork.CompleteAsync();

 			await transaction.CommitAsync();

			return booking.ToDto();
		}
		catch
		{
 			await transaction.RollbackAsync();
			throw;
		}
	}

	public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id)
	{
		var booking = await bookingRepository.GetByIdAsync(id);

		if (booking == null) return null;

		return booking.ToDto();
	}

	public async Task<bool> CheckAvailabilityAsync(string resourceId, DateTime startTime, DateTime endTime)
	{
		if (startTime >= endTime)
		{
			throw new ArgumentException("Start time must be earlier than end time.");
		}

		return await bookingRepository.IsResourceAvailableAsync(resourceId, startTime, endTime);
	}

	public async Task<PagedResult<BookingResponseDto>> GetAllBookingsAsync(PaginationParams paginationParams)
	{
		var pagedBookings = await bookingRepository.GetPagedAsync(
				paginationParams.PageNumber,
				paginationParams.PageSize);

		return pagedBookings.MapToPagedResult(MapToResponseDto);
	}

	public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(GetUserBookingsQueryDto queryDto)
	{
		var pagedBookings = await bookingRepository.GetBookingsByUserIdAsync(
			queryDto.UserId,
			queryDto.Status,
			trackChanges: false,
			queryDto.PageNumber,
			queryDto.PageSize);

		return pagedBookings.Items.Select(MapToResponseDto
		).ToList();
	}

	public async Task<BookingResponseDto> UpdateBookingStatusAsync(Guid id, BookingStatus newStatus)
	{
		var booking = await bookingRepository.GetByIdAsync(id, trackChanges: true);

		if (booking is null)
		{
			throw new KeyNotFoundException("Booking not found.");
		}

		if (newStatus == BookingStatus.Canceled)
		{
			booking.Cancel();
		}
		else
		{
			booking.Status = newStatus;
		}

		await unitOfWork.CompleteAsync();

		return MapToResponseDto(booking);
	}

	public async Task<PagedResult<BookingResponseDto>> GetBookingsByResourceWithDateRangeAsync(
	string resourceId,
	GetResourceBookingsFilterDto filter)
	{
		var pagedBookings = await bookingRepository.GetBookingsByResourceWithDateRangeAsync(
		resourceId,
		filter.StartDate,
		filter.EndDate,
		filter.PageNumber,
		filter.PageSize,
		filter.Status,
		trackChanges: false);

		var itemDtos = pagedBookings.Items.Select(b => new BookingResponseDto
		(
			 b.Id,
			b.ResourceId,
			b.UserId,
			b.StartDateTime,
			b.EndDateTime,
			 b.Status.ToString()
		)).ToList();

		return new PagedResult<BookingResponseDto>(
			itemDtos,
			pagedBookings.TotalCount,
			pagedBookings.PageNumber,
			pagedBookings.PageSize);
	}

	private static BookingResponseDto MapToResponseDto(Booking booking)
	{
		return new BookingResponseDto(
			booking.Id,
			booking.ResourceId,
			booking.UserId,
			booking.StartDateTime,
			booking.EndDateTime,
			booking.Status.ToString()
		);
	}

}