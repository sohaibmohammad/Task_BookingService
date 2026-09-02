using BackendBookingManagement.Application.src.DTOs;
using BackendBookingManagement.Application.src.DTOs.Resources;

namespace BackendBookingManagement.Application.src.Interfaces.Services
{
	public interface IResourceService
	{
		Task<IEnumerable<ResourceDto>> GetAllResourcesAsync(CancellationToken cancellationToken = default);
		Task<ResourceDto?> GetResourceByIdAsync(string id, CancellationToken cancellationToken = default);
		Task<ResourceDto> AddResourceAsync(CreateResourceDto createDto, CancellationToken cancellationToken = default);
	}
}