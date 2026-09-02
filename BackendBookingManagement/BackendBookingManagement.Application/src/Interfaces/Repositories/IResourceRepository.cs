using BackendBookingManagement.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Text;
namespace BackendBookingManagement.Application.src.Interfaces.Repositories;

public interface IResourceRepository : IRepository<Resource, string>
{
}
