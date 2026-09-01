using BackendBookingManagement.Application.src.Common;

namespace BackendBookingManagement.Application.src.Mappings;

public static class PagedResultExtensions
{
	public static PagedResult<TDestination> MapToPagedResult<TSource, TDestination>(
		this PagedResult<TSource> pagedSource,
		Func<TSource, TDestination> mapper)
	{
		var mappedItems = pagedSource.Items.Select(mapper).ToList();

		return new PagedResult<TDestination>(
			mappedItems,
			pagedSource.TotalCount,
			pagedSource.PageNumber,
			pagedSource.PageSize);
	}
}
