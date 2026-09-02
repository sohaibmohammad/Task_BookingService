using BackendBookingManagement.Application.src.Common;
using BackendBookingManagement.Application.src.DTOs.Bookings;
using BackendBookingManagement.Application.src.Interfaces.Repositories;
using BackendBookingManagement.Application.src.Services;
using BackendBookingManagement.Domain.src.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Tests.Services;

public class BookingServiceTest
{
	private readonly Mock<IBookingRepository> _bookingRepoMock;
	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<IDbContextTransaction> _transactionMock;
	private readonly BookingService _sut;

	public BookingServiceTest()
	{
		_bookingRepoMock = new Mock<IBookingRepository>();
		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_transactionMock = new Mock<IDbContextTransaction>();

		_unitOfWorkMock.Setup(u => u.Bookings).Returns(_bookingRepoMock.Object);
		_unitOfWorkMock.Setup(u => u.BeginTransactionAsync(default))
			.ReturnsAsync(_transactionMock.Object);

		_sut = new BookingService(_bookingRepoMock.Object, _unitOfWorkMock.Object);
	}




	[Fact]
	public async Task CreateBookingAsync_ShouldCreateBookingAndCommit_WhenResourceIsAvailable()
	{
		var resourceIdStr = Guid.NewGuid().ToString();
		var userIdStr = Guid.NewGuid().ToString();

		var dto = new CreateBookingDto(
				resourceIdStr,
				userIdStr,
				DateTime.UtcNow.AddHours(1),
				DateTime.UtcNow.AddHours(2)
			);

		_bookingRepoMock.Setup(r => r.IsResourceAvailableAsync(dto.ResourceId, dto.StartDateTime, dto.EndDateTime))
			.ReturnsAsync(true);

		_bookingRepoMock.Setup(r => r.AddAsync(It.IsAny<Booking>()))
			.Returns(Task.CompletedTask);
		                  
		_unitOfWorkMock.Setup(u => u.CompleteAsync(default))
			.ReturnsAsync(1);

		var result = await _sut.CreateBookingAsync(dto);

		result.Should().NotBeNull();
		result.ResourceId.Should().Be(dto.ResourceId);
		result.UserId.Should().Be(dto.UserId);

		_transactionMock.Verify(t => t.CommitAsync(default), Times.Once);
	}


	//===================================================================================================================================

	[Fact]
	public async Task CreateBookingAsync_ShouldHandleConcurrentBookings_AndPreventDoubleBooking()
	{
 		var resourceIdStr = Guid.NewGuid().ToString();
		var dto1 = new CreateBookingDto(resourceIdStr, "user-1", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
		var dto2 = new CreateBookingDto(resourceIdStr, "user-2", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

  		_bookingRepoMock.SetupSequence(r => r.IsResourceAvailableAsync(resourceIdStr, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
			.ReturnsAsync(true)   
			.ReturnsAsync(false); 

		_bookingRepoMock.Setup(r => r.AddAsync(It.IsAny<Booking>()))
			.Returns(Task.CompletedTask);

		_unitOfWorkMock.Setup(u => u.CompleteAsync(default))
			.ReturnsAsync(1);

 		var task1 = _sut.CreateBookingAsync(dto1);
		var task2 = _sut.CreateBookingAsync(dto2);

 		var execution = async () => await Task.WhenAll(task1, task2);


 		await execution.Should().ThrowAsync<InvalidOperationException>();
	}



	//===================================================================================================================================

	[Fact]
	public async Task CreateBookingAsync_ShouldThrowException_WhenResourceIsUnavailable()
	{
 		var dto = new CreateBookingDto(
			ResourceId: Guid.NewGuid().ToString(),
			UserId: Guid.NewGuid().ToString(),
			StartDateTime: DateTime.UtcNow.AddHours(1),
			EndDateTime: DateTime.UtcNow.AddHours(2)
		);

 		_bookingRepoMock.Setup(r => r.IsResourceAvailableAsync(dto.ResourceId, dto.StartDateTime, dto.EndDateTime))
			.ReturnsAsync(false);

 		Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

 		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*resource is unavailable*");

 		_bookingRepoMock.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never);
		_unitOfWorkMock.Verify(u => u.CompleteAsync(default), Times.Never);
		_transactionMock.Verify(t => t.CommitAsync(default), Times.Never);
	}




	//===================================================================================================================================


	[Fact]
	public async Task CancelBookingAsync_ShouldThrowException_WhenBookingNotFound()
	{
 		var nonExistentId = Guid.NewGuid();

 		_bookingRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
			.ReturnsAsync((Booking?)null);

 		Func<Task> act = async () => await _sut.CancelBookingAsync(nonExistentId);

 		await act.Should().ThrowAsync<KeyNotFoundException>()
			.WithMessage("*not available*");

		_bookingRepoMock.Verify(r => r.Update(It.IsAny<Booking>()), Times.Never);
	}





	//===================================================================================================================================

	[Fact]
	public async Task CancelBookingAsync_ShouldThrowException_WhenBookingIsAlreadyCanceled()
	{
 		var bookingId = Guid.NewGuid();
		var canceledBooking = new Booking
		{
			Id = bookingId,
			ResourceId = Guid.NewGuid().ToString(),
			UserId = Guid.NewGuid().ToString(),
			StartDateTime = DateTime.UtcNow.AddHours(2),
			EndDateTime = DateTime.UtcNow.AddHours(3),
			Status = BookingStatus.Canceled  
		};

		_bookingRepoMock.Setup(r => r.GetByIdAsync(bookingId))
			.ReturnsAsync(canceledBooking);

 		Func<Task> act = async () => await _sut.CancelBookingAsync(bookingId);

 		await act.Should().ThrowAsync<InvalidOperationException>();

		_bookingRepoMock.Verify(r => r.Update(It.IsAny<Booking>()), Times.Never);
		_unitOfWorkMock.Verify(u => u.CompleteAsync(default), Times.Never);
	}




	//===================================================================================================================================

	[Fact]
	public async Task CancelBookingAsync_ShouldCancelSuccessfully_WhenBookingIsValid()
	{
 		var bookingId = Guid.NewGuid();
		var validBooking = new Booking
		{
			Id = bookingId,
			ResourceId = Guid.NewGuid().ToString(),
			UserId = Guid.NewGuid().ToString(),
			StartDateTime = DateTime.UtcNow.AddHours(2),  
			EndDateTime = DateTime.UtcNow.AddHours(3),
			Status = BookingStatus.Confirmed
		};

		_bookingRepoMock.Setup(r => r.GetByIdAsync(bookingId))
			.ReturnsAsync(validBooking);

		_unitOfWorkMock.Setup(u => u.CompleteAsync(default))
			.ReturnsAsync(1);

 		await _sut.CancelBookingAsync(bookingId);

 		validBooking.Status.Should().Be(BookingStatus.Canceled);
		_bookingRepoMock.Verify(r => r.Update(validBooking), Times.Once);
		_unitOfWorkMock.Verify(u => u.CompleteAsync(default), Times.Once);
	}



	//===================================================================================================================================


	[Fact]
	public async Task GetBookingByIdAsync_ShouldReturnDto_WhenBookingExists()
	{
		var bookingId = Guid.NewGuid();
		var booking = new Booking
		{
			Id = bookingId,
			ResourceId = Guid.NewGuid().ToString(),
			UserId = Guid.NewGuid().ToString(),
			StartDateTime = DateTime.UtcNow.AddHours(1),
			EndDateTime = DateTime.UtcNow.AddHours(2),
			Status = BookingStatus.Confirmed
		};

		_bookingRepoMock.Setup(r => r.GetByIdAsync(bookingId))
			.ReturnsAsync(booking);

		var result = await _sut.GetBookingByIdAsync(bookingId);

		result.Should().NotBeNull();
		result.Id.Should().Be(bookingId);
	}
	[Fact]
	public async Task GetBookingsByResourceWithDateRangeAsync_WhenRepositoryReturnsBookings_ShouldReturnPagedResult()
	{
 		var resourceId = "resource-123";
		var filter = new GetResourceBookingsFilterDto
		{
			PageNumber = 1,
			PageSize = 10,
			StartDate = null,
			EndDate = null,
			Status = null
		};

		var bookingId = Guid.NewGuid();
		var bookingFromRepo = new Booking
		{
			Id = bookingId,
			ResourceId = resourceId,
			UserId = "user-456",
			StartDateTime = DateTime.UtcNow.AddDays(1),
			EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
			Status = BookingStatus.Confirmed
		};

		var pagedResultFromRepo = new PagedResult<Booking>(
			new List<Booking> { bookingFromRepo },
			totalCount: 1,
			pageNumber: 1,
			pageSize: 10
		);

		var mockRepo = new Mock<IBookingRepository>();
		var mockUnitOfWork = new Mock<IUnitOfWork>();

		mockRepo.Setup(r => r.GetBookingsByResourceWithDateRangeAsync(
			resourceId,
			filter.StartDate,
			filter.EndDate,
			filter.PageNumber,
			filter.PageSize,
			filter.Status,
			false))
			.ReturnsAsync(pagedResultFromRepo);

		var service = new BookingService(mockRepo.Object, mockUnitOfWork.Object);

 		var result = await service.GetBookingsByResourceWithDateRangeAsync(resourceId, filter);

 		result.Should().NotBeNull();
		result.TotalCount.Should().Be(1);
		result.PageNumber.Should().Be(1);
		result.PageSize.Should().Be(10);
		result.Items.Should().HaveCount(1);

		var item = result.Items.First(); 
		item.Id.Should().Be(bookingId);
		item.ResourceId.Should().Be(resourceId);
		item.Status.Should().Be("Confirmed");
	}
}
