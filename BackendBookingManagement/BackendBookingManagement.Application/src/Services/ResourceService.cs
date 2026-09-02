using BackendBookingManagement.Application.src.DTOs;
using BackendBookingManagement.Application.src.DTOs.Resources;
using BackendBookingManagement.Application.src.Interfaces.Repositories;
using BackendBookingManagement.Application.src.Interfaces.Services;
using BackendBookingManagement.Domain.src.Entity;
using Microsoft.Extensions.Caching.Memory;

namespace BackendBookingManagement.Application.src.Services;

public class ResourceService(IResourceRepository resourceRepository ,IUnitOfWork unitOfWork, IMemoryCache memoryCache) : IResourceService
{
	private readonly IResourceRepository _resourceRepository = resourceRepository;
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMemoryCache _memoryCache = memoryCache;
	private const string CacheKey = "resources_cache_key";

	public async Task<IEnumerable<ResourceDto>> GetAllResourcesAsync(CancellationToken cancellationToken = default)
	{
 		if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<ResourceDto>? cachedResources) && cachedResources != null)
		{
			return cachedResources;
		}

		var resources = await _resourceRepository.GetAllAsync(trackChanges: false, cancellationToken);
		var resourceDtos = resources.Select(r => new ResourceDto(r.Id, r.Name, r.Description)).ToList();

 		var cacheOptions = new MemoryCacheEntryOptions()
			.SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
			.SetSlidingExpiration(TimeSpan.FromMinutes(2));

		_memoryCache.Set(CacheKey, resourceDtos, cacheOptions);

		return resourceDtos;
	}

	public async Task<ResourceDto?> GetResourceByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var resource = await _resourceRepository.GetByIdAsync(id, trackChanges: false, cancellationToken);
		if (resource == null) return null;

		return new ResourceDto(resource.Id, resource.Name, resource.Description);
	}

	public async Task<ResourceDto> AddResourceAsync(CreateResourceDto createDto, CancellationToken cancellationToken = default)
	{
 		var resource = new Resource
		{
			Name = createDto.Name,
			Description = createDto.Description
		};

		await _resourceRepository.AddAsync(resource, cancellationToken);
		await _unitOfWork.CompleteAsync(cancellationToken);

		_memoryCache.Remove(CacheKey);

 		return new ResourceDto(resource.Id, resource.Name, resource.Description);
	}
}