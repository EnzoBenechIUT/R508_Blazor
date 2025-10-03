using App.Mapper;
using App.DTO;
using App.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using App.Models;
[Route("api/marques/[action]")]
[ApiController]
public class MarqueController(
    IMapper<Marque, MarqueDto> marqueMapperDTO,
    IDataRepository<Marque> manager
) : ControllerBase
{
    [HttpGet("{id}")]
    [ActionName("GetById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarqueDto?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result.Value == null ? NotFound() : marqueMapperDTO.ToDTO(result.Value);
    }

    [HttpDelete("{id}")]
    [ActionName("Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var marque = await manager.GetByIdAsync(id);
        if (marque.Value == null) return NotFound();

        await manager.DeleteAsync(marque.Value);
        return NoContent();
    }

    [HttpGet]
    [ActionName("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<MarqueDto>>> GetAll()
    {
        var marques = (await manager.GetAllAsync()).Value;
        if (marques == null || !marques.Any()) return NoContent();

        var dtos = marqueMapperDTO.ToDTOs(marques);
        return Ok(dtos);
    }

    [HttpPost]
    [ActionName("Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MarqueDto>> Create([FromBody] MarqueDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var marque = marqueMapperDTO.ToEntity(dto);
        await manager.AddAsync(marque);

        var retourDto = marqueMapperDTO.ToDTO(marque);
        return CreatedAtAction("GetById", new { id = marque.IdMarque }, retourDto);
    }

    [HttpPut("{id}")]
    [ActionName("Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] MarqueDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var marqueToUpdate = await manager.GetByIdAsync(id);
        if (marqueToUpdate.Value == null) return NotFound();

        var marque = marqueMapperDTO.ToEntity(dto);
        await manager.UpdateAsync(marqueToUpdate.Value, marque);
        return NoContent();
    }
}
