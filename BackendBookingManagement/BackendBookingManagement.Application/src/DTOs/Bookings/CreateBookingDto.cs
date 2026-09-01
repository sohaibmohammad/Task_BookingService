using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Application.src.DTOs.Bookings;

public record CreateBookingDto(
	string ResourceId,
	string UserId,
	DateTime StartDateTime,
	DateTime EndDateTime
);
