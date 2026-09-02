using BackendBookingManagement.Application.src.Interfaces.Repositories;
using BackendBookingManagement.Domain.src.Entity;
using BackendBookingManagement.Infrastructure.src.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendBookingManagement.Infrastructure.src.Repositories;

public class ResourceRepository(AppDbContext context) : Repository<Resource, string>(context), IResourceRepository
{
}
