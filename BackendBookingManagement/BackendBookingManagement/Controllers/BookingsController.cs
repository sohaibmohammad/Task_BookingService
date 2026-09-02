using BackendBookingManagement.Application.src.Common;
using BackendBookingManagement.Application.src.DTOs.Bookings;
using BackendBookingManagement.Application.src.Interfaces.Services;
using BackendBookingManagement.Application.src.Services;
using BackendBookingManagement.Domain.src.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BackendBookingManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto dto)
	{
		var result = await bookingService.CreateBookingAsync(dto);
		return StatusCode(StatusCodes.Status201Created, result);
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<BookingResponseDto>> GetById(Guid id)
	{
		var booking = await bookingService.GetBookingByIdAsync(id);

		if (booking == null)
		{
			return NotFound(new { message = "Booking not found." });
		}

		return Ok(booking);
	}

	[HttpPut("{id:guid}/cancel")]
	public async Task<IActionResult> CancelBooking(Guid id)
	{
		await bookingService.CancelBookingAsync(id);
		return NoContent(); // 204 Success with no content
	}

	[HttpGet]
	public async Task<ActionResult<PagedResult<BookingResponseDto>>> GetAll([FromQuery] PaginationParams paginationParams)
	{
		var pagedResult = await bookingService.GetAllBookingsAsync(paginationParams);
		return Ok(pagedResult);
	}

	[HttpGet("user")]
	public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUserBookings([FromQuery] GetUserBookingsQueryDto queryDto)
	{
		var bookings = await bookingService.GetUserBookingsAsync(queryDto);
		return Ok(bookings);
	}

	[HttpGet("check-availability")]
	public async Task<ActionResult<bool>> CheckAvailability([FromQuery] string resourceId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
	{
		var isAvailable = await bookingService.CheckAvailabilityAsync(resourceId, startTime, endTime);
		return Ok(new { isAvailable });
	}

	[HttpPatch("{id:guid}/status")]
	public async Task<ActionResult<BookingResponseDto>> UpdateStatus(Guid id, [FromBody] BookingStatus newStatus)
	{
		var updatedBooking = await bookingService.UpdateBookingStatusAsync(id, newStatus);
		return Ok(updatedBooking);
	}

	[HttpGet("availability/slots")]
	public async Task<ActionResult<IEnumerable<BookedTimeSlotDto>>> GetBookedTimeSlots(
		[FromQuery] string resourceId,
		[FromQuery] DateTime date)
	{
		var slots = await bookingService.GetBookedTimeSlotsAsync(resourceId, date);
		return Ok(slots);
	}
	[HttpGet("resource/{resourceId}")]
	public async Task<ActionResult<PagedResult<BookingResponseDto>>> GetBookingsByResource(
		string resourceId,
		[FromQuery] GetResourceBookingsFilterDto filter)
	{
		var pagedResult = await bookingService.GetBookingsByResourceWithDateRangeAsync(resourceId, filter);
		return Ok(pagedResult);
	}
}
