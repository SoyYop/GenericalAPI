using GenericalAPI.Application.Abstractions.Mappings;
using GenericalAPI.Application.Contracts.Stores;
using GenericalAPI.Domain.Entities;

namespace GenericalAPI.Application.Mappings.Stores;

public sealed class StoreMapper : IEntityMapper<Store, StoreDto>
{
    public StoreDto ToDto(Store entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Products = entity.Products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                StoreId = p.StoreId
            }).ToList()
        };

    public Store FromDto(StoreDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Products = dto.Products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                StoreId = p.StoreId
            }).ToList()
        };

    public void MapToEntity(StoreDto dto, Store entity)
    {
        entity.Name = dto.Name;

        // Replace products for simplicity; adjust if you need partial updates.
        entity.Products = dto.Products.Select(p => new Product
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            StoreId = p.StoreId
        }).ToList();
    }
}
