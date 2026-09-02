namespace BackendBookingManagement.Application.src.DTOs.Bookings;

public class GetResourceBookingsQueryDto
{
	public Guid ResourceId { get; set; }
	public DateTime? StartDate { get; set; }
	public DateTime? EndDate { get; set; }
	public int PageNumber { get { return _pageNumber; } set { _pageNumber = (value < 1) ? 1 : value; } }
	private int _pageNumber = 1;

	public int PageSize { get { return _pageSize; } set { _pageSize = (value > 50) ? 50 : (value < 1) ? 10 : value; } }
	private int _pageSize = 10;

	public string SortBy { get; set; } = "StartDateTime"; // حقل الترتيب الافتراضي
	public bool Ascending { get; set; } = true; // اتجاه الترتيب (تصاعدي أو تنازلي)
}