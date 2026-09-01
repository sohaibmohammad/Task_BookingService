namespace BackendBookingManagement.Application.src.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
	IBookingRepository Bookings { get; }
	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
