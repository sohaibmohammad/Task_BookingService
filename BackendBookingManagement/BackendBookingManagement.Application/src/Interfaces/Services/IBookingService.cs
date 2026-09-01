using BackendBookingManagement.Application.src.Common;
using BackendBookingManagement.Application.src.DTOs.Bookings;
using BackendBookingManagement.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Application.src.Interfaces.Services
{
	public interface IBookingService
	{
		Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
		Task<BookingResponseDto?> GetBookingByIdAsync(Guid id);
		Task CancelBookingAsync(Guid id);

 		Task<PagedResult<BookingResponseDto>> GetAllBookingsAsync(PaginationParams paginationParams);

		Task<BookingResponseDto> UpdateBookingStatusAsync(Guid id, BookingStatus newStatus);

		Task<bool> CheckAvailabilityAsync(Guid resourceId, DateTime startTime, DateTime endTime);
		Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(GetUserBookingsQueryDto queryDto);
	}
}
