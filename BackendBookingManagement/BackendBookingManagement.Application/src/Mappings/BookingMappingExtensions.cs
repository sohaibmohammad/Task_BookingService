using BackendBookingManagement.Application.src.DTOs.Bookings;
using BackendBookingManagement.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Application.src.Mappings;

public static class BookingMappingExtensions
{
	public static BookingResponseDto ToDto(this Booking booking)
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
