using BackendBookingManagement.Domain.src.Entity;

namespace BackendBookingManagement.Application.src.DTOs.Bookings;

public record GetUserBookingsQueryDto(
	string UserId,
	BookingStatus? Status = null,
	int PageNumber = 1,
	int PageSize = 50
);