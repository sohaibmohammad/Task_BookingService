namespace BackendBookingManagement.Application.src.DTOs.Bookings;

public record BookingResponseDto(
	Guid Id,
	string ResourceId,
	string UserId,
	DateTime StartDateTime,
	DateTime EndDateTime,
	string Status
);
