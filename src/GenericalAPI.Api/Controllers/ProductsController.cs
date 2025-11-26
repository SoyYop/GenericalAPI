using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Api.Controllers;

public sealed class ProductsController : CrudController<ProductDto, Product>
{
    public ProductsController(ICrudService<ProductDto, Product> service) : base(service)
    {
    }
}
