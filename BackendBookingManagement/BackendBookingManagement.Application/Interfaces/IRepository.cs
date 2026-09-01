using BackendBookingManagement.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Application.Interfaces;

public interface IRepository<T,TKey> where T : class
{
	Task<T?> GetByIdAsync(TKey id, bool trackChanges = false, CancellationToken cancellationToken = default);

	Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

	Task AddAsync(T entity, CancellationToken cancellationToken = default);
	void Update(T entity);
	void Delete(T entity);
}
