using BackendBookingManagement.Application.src.DTOs;
using BackendBookingManagement.Application.src.DTOs.Resources;
using BackendBookingManagement.Application.src.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendBookingManagement.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ResourcesController(IResourceService resourceService) : ControllerBase
	{
		private readonly IResourceService _resourceService = resourceService;

		[HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var resources = await _resourceService.GetAllResourcesAsync(cancellationToken);
			return Ok(resources);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
		{
			var resource = await _resourceService.GetResourceByIdAsync(id, cancellationToken);
			if (resource == null) return NotFound();

			return Ok(resource);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateResourceDto createDto, CancellationToken cancellationToken)
		{
			var createdResource = await _resourceService.AddResourceAsync(createDto, cancellationToken);
			return CreatedAtAction(nameof(GetById), new { id = createdResource.Id }, createdResource);
		}
	}
}