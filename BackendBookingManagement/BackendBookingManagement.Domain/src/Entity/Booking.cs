using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Domain.src.Entity;
	public class Booking
	{
		public string ResourceId { get; set; } = string.Empty;

		public Guid Id { get; set; } = Guid.NewGuid();
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
		public Resource? Resource { get; set; }

		public string UserId { get; set; } = string.Empty;
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
		public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

		public void Cancel()
		{
		if(DateTime.UtcNow>= StartDateTime)
			throw new InvalidOperationException("Cannot cancel a booking that has already started or passed.");
		Status = BookingStatus.Canceled;
		}
	}
