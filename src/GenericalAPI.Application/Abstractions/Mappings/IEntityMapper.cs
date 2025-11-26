namespace GenericalAPI.Application.Abstractions.Mappings;

public interface IEntityMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);
    TEntity FromDto(TDto dto);
    void MapToEntity(TDto dto, TEntity entity);
}
