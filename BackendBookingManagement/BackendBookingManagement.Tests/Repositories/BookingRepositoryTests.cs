using BackendBookingManagement.Domain.src.Entity;
using BackendBookingManagement.Infrastructure.src.Database;
using BackendBookingManagement.Infrastructure.src.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace BackendBookingManagement.Tests.Repositories;

public class BookingRepositoryTests
{
	private readonly AppDbContext _context;
	private readonly BookingRepository _repository;
	public BookingRepositoryTests()
	{
		// إعداد قاعدة بيانات وهمية في الذاكرة لكل اختبار لضمان العزل
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // اسم عشوائي حتى لا تتداخل البيانات بين الاختبارات
			.Options;

		_context = new AppDbContext(options);
		_repository = new BookingRepository(_context);
	}
	[Fact]
	public async Task IsResourceAvailableAsync_WhenConflictingBookingExists_ShouldReturnFalse()
	{
		var resourceId = "res-1";
		var startTime = new DateTime(2026, 6, 1, 10, 0, 0);
		var endTime = new DateTime(2026, 6, 1, 12, 0, 0);

		var existingBooking = new Booking
		{
			Id = Guid.NewGuid(),
			ResourceId = resourceId,
			UserId = "user-1",
			StartDateTime = startTime,
			EndDateTime = endTime,
			Status = BookingStatus.Confirmed
		};
		_context.Bookings.Add(existingBooking);
		await _context.SaveChangesAsync();

		var isAvailable = await _repository.IsResourceAvailableAsync(
			resourceId,
			startTime.AddHours(1),
			endTime.AddHours(1)
		);
		isAvailable.Should().BeFalse();
	}


	[Fact]
	public async Task GetByIdWithResourceAsync_WhenBookingExists_ShouldReturnBooking()
	{

		var bookingId = Guid.NewGuid();
		var resourceId = "res-99";

 		var resource = new Resource
		{
			Id = resourceId,
			Name = "Test Resource"
 		};
		_context.Resources.Add(resource);

 		var booking = new Booking
		{
			Id = bookingId,
			ResourceId = resourceId,
			UserId = "user-99",
			StartDateTime = DateTime.UtcNow.AddDays(2),
			EndDateTime = DateTime.UtcNow.AddDays(2).AddHours(3),
			Status = BookingStatus.Confirmed
		};

		_context.Bookings.Add(booking);
		await _context.SaveChangesAsync();

 		var result = await _repository.GetByIdWithResourceAsync(bookingId);

 		result.Should().NotBeNull();
		result.Id.Should().Be(bookingId);
		result.ResourceId.Should().Be(resourceId);
	}

	[Fact]
	public async Task GetBookingsByUserIdAsync_ShouldReturnPagedResultsForSpecificUser()
	{
 		var targetUserId = "user-123";

 		for (int i = 1; i <= 3; i++)
		{
			_context.Bookings.Add(new Booking
			{
				Id = Guid.NewGuid(),
				ResourceId = $"res-{i}",
				UserId = targetUserId,
				StartDateTime = DateTime.UtcNow.AddDays(i),
				EndDateTime = DateTime.UtcNow.AddDays(i).AddHours(1),
				Status = BookingStatus.Confirmed
			});
		}
 		_context.Bookings.Add(new Booking
		{
			Id = Guid.NewGuid(),
			ResourceId = "res-other",
			UserId = "other-user",
			StartDateTime = DateTime.UtcNow.AddDays(1),
			EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(1),
			Status = BookingStatus.Confirmed
		});

		await _context.SaveChangesAsync();

 		var pagedResult = await _repository.GetBookingsByUserIdAsync(targetUserId, pageNumber: 1, pageSize: 2);

 		pagedResult.Should().NotBeNull();
		pagedResult.Items.Should().HaveCount(2);  
		pagedResult.TotalCount.Should().Be(3);  
		pagedResult.Items.All(b => b.UserId == targetUserId).Should().BeTrue();  
	}

	[Fact]
	public async Task GetBookingByStatusAsync_ShouldReturnPagedResultsForSpecificStatus()
	{
 		var targetStatus = BookingStatus.Completed;

 		for (int i = 1; i <= 2; i++)
		{
			_context.Bookings.Add(new Booking
			{
				Id = Guid.NewGuid(),
				ResourceId = $"res-p-{i}",
				UserId = "user-1",
				StartDateTime = DateTime.UtcNow.AddDays(i),
				EndDateTime = DateTime.UtcNow.AddDays(i).AddHours(1),
				Status = targetStatus
			});
		}
 		_context.Bookings.Add(new Booking
		{
			Id = Guid.NewGuid(),
			ResourceId = "res-c-1",
			UserId = "user-1",
			StartDateTime = DateTime.UtcNow.AddDays(5),
			EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(1),
			Status = BookingStatus.Confirmed
		});

		await _context.SaveChangesAsync();

 		var pagedResult = await _repository.GetBookingByStatusAsync(targetStatus, pageNumber: 1, pageSize: 10);

 		pagedResult.Should().NotBeNull();
		pagedResult.Items.Should().HaveCount(2);  
		pagedResult.TotalCount.Should().Be(2);
		pagedResult.Items.All(b => b.Status == targetStatus).Should().BeTrue();
	}
}



