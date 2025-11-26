using GenericalAPI.Application.Abstractions.Mappings;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Application.Mappings.Products;

public sealed class ProductMapper : IEntityMapper<Product, ProductDto>
{
    public ProductDto ToDto(Product entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            Description = entity.Description,
            StoreId = entity.StoreId
        };

    public Product FromDto(ProductDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            StoreId = dto.StoreId
        };

    public void MapToEntity(ProductDto dto, Product entity)
    {
        entity.Name = dto.Name;
        entity.Price = dto.Price;
        entity.Description = dto.Description;
        entity.StoreId = dto.StoreId;
    }
}
