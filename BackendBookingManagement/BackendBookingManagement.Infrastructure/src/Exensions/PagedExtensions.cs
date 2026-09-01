namespace BackendBookingManagement.Infrastructure.src.Exensions
{
	public static class PagedExtensions {
		public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
		{
			return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
		}

	}
}
