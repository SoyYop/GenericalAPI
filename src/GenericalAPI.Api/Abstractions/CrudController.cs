using GenericalAPI.Application.Abstractions.Persistence;
using GenericalAPI.Domain.Abstractions;
using GenericalAPI.Shared.Contracts.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace GenericalAPI.Api.Abstractions;

[ApiController]
[Route("api/[controller]")]
public abstract class CrudController<TDto, TEntity> : ControllerBase
    where TEntity : IAuditableEntity, new()
{
    private readonly ICrudService<TDto, TEntity> _service;

    protected CrudController(ICrudService<TDto, TEntity> service)
    {
        _service = service;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetAsync(id, cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public Task<PagedResult<TDto>> GetPaged([FromQuery] PagedRequest request, CancellationToken cancellationToken) =>
        _service.GetPagedAsync(request, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<long>> Create([FromBody] TDto dto, CancellationToken cancellationToken)
    {
        var user = User.Identity?.Name ?? "system";
        var id = await _service.CreateAsync(dto, user, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] TDto dto, CancellationToken cancellationToken)
    {
        var user = User.Identity?.Name ?? "system";
        await _service.UpdateAsync(id, dto, user, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
