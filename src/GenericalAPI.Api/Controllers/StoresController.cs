using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Api.Controllers;

public sealed class StoresController : CrudController<StoreDto, Store>
{
    public StoresController(ICrudService<StoreDto, Store> service) : base(service)
    {
    }
}
