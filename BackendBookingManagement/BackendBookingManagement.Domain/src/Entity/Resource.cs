namespace BackendBookingManagement.Domain.src.Entity;

public class Resource
{
	public string Id { get; set; } = string.Empty; 
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }

	public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}