using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Exensions
{
	public static class QueryableExtensions
	{
		
		public static IQueryable<T> ApplyTracking<T>(this IQueryable<T> query, bool trackChanges) where T : class
		{
			return trackChanges ? query : query.AsNoTracking();
		}
	}
}
