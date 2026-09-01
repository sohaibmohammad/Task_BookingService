using BackendBookingManagement.Application.Common;
using BackendBookingManagement.Application.Interfaces;
using BackendBookingManagement.Domain.src.Entity;
using BackendBookingManagement.Infrastructure.src.Database;
using BackendBookingManagement.Infrastructure.src.Exensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Repositories;

public class Repository<T,TKey>(AppDbContext context) : IRepository<T,TKey> where T : class
{
	protected readonly AppDbContext _context = context;
	private readonly DbSet<T> _dbSet = context.Set<T>();
	public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		await _dbSet.AddAsync(entity, cancellationToken);
	}

	public void Update(T entity)
	{
		_dbSet.Update(entity);
	}

	public void Delete(T entity)
	{
		_dbSet.Remove(entity);
	}

	public async Task<T?> GetByIdAsync(TKey id, bool trackChanges=false,CancellationToken cancellationToken = default)
	{
		return await _dbSet
				.ApplyTracking(trackChanges)
				.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id), cancellationToken);
	}

	public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	{
		var totalCount = await _dbSet.CountAsync(cancellationToken);
		var query = _dbSet.ApplyTracking(false);
		var items = PagedExtensions.ApplyPaging(query, pageNumber, pageSize);

		return new PagedResult<T>(items, totalCount, pageNumber, pageSize);

	}

 
}