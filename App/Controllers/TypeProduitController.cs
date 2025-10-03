using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("api/typeproduits/[action]")]
[ApiController]
public class TypeProduitController(
    IMapper<TypeProduit, TypeProduitDto> mapper,
    IDataRepository<TypeProduit> manager
) : ControllerBase
{
    [HttpGet("{id}")]
    [ActionName("GetById")]
    public async Task<ActionResult<TypeProduitDto?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result.Value == null ? NotFound() : mapper.ToDTO(result.Value);
    }

    [HttpGet]
    [ActionName("GetAll")]
    public async Task<ActionResult<IEnumerable<TypeProduitDto>>> GetAll()
    {
        var entities = (await manager.GetAllAsync()).Value;
        return Ok(mapper.ToDTOs(entities));
    }

    [HttpPost]
    [ActionName("Create")]
    public async Task<ActionResult<TypeProduitDto>> Create([FromBody] TypeProduitDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = mapper.ToEntity(dto);
        await manager.AddAsync(entity);
        var createdDto = mapper.ToDTO(entity);

        return CreatedAtAction("GetById", new { id = createdDto.IdTypeProduit }, createdDto);
    }

    [HttpPut("{id}")]
    [ActionName("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] TypeProduitDto dto)
    {
        if (id != dto.IdTypeProduit) return BadRequest();

        var existing = await manager.GetByIdAsync(id);
        if (existing.Value == null) return NotFound();

        var entity = mapper.ToEntity(dto);
        await manager.UpdateAsync(existing.Value, entity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ActionName("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await manager.GetByIdAsync(id);
        if (existing.Value == null) return NotFound();

        await manager.DeleteAsync(existing.Value);
        return NoContent();
    }
}
