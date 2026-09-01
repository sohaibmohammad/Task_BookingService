using BackendBookingManagement.Application.src.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Application.src.Interfaces.Repositories;

public interface IRepository<T,TKey> where T : class
{
	Task<T?> GetByIdAsync(TKey id, bool trackChanges = false, CancellationToken cancellationToken = default);

	Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

	Task AddAsync(T entity, CancellationToken cancellationToken = default);
	void Update(T entity);
	void Delete(T entity);
}
