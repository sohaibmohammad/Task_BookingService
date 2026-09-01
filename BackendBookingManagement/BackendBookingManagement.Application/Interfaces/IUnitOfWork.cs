namespace BackendBookingManagement.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
	IBookingRepository Bookings { get; }
	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
