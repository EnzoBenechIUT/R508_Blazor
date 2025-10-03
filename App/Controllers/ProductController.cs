using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("api/produits/[action]")]
[ApiController]
public class ProductController(IMapper<Produit, ProduitDto> produitMapperDTO, IDataRepository<Produit> manager) : ControllerBase
{
    [HttpGet("{id}")]
    [ActionName("GetById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProduitDto?>> Get(int id)
    {
        var result = await manager.GetByIdAsync(id);
        return result.Value == null ? NotFound() : produitMapperDTO.ToDTO(result.Value);
    }

    [HttpDelete("{id}")]
    [ActionName("Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        ActionResult<Produit?> produit = await manager.GetByIdAsync(id);
        
        if (produit.Value == null)
            return NotFound();
        
        await manager.DeleteAsync(produit.Value);
        return NoContent();
    }

    [HttpGet]
    [ActionName("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
    {
        // Récupération de tous les produits avec leurs relations
        var produits = await manager.GetAllAsync();

        if (produits?.Value == null || !produits.Value.Any())
            return NoContent();

        // Conversion en DTO
        var dtos = produitMapperDTO.ToDTOs(produits.Value);

        return Ok(dtos);
    }

    [HttpPost]
    [ActionName("Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProduitDto>> Create([FromBody] ProduitDto produitDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var produit = produitMapperDTO.ToEntity(produitDto); // Convert DTO -> Entity
        await manager.AddAsync(produit);

        var dto = produitMapperDTO.ToDTO(produit); // Convert back pour retourner
        return CreatedAtAction("GetById", new { id = produit.IdProduit }, dto);
    }

    [HttpPut("{id}")]
    [ActionName("Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ProduitDto produitDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var produitToUpdate = await manager.GetByIdAsync(id);
        if (produitToUpdate.Value == null)
            return NotFound();

        var produit = produitMapperDTO.ToEntity(produitDto);
        await manager.UpdateAsync(produitToUpdate.Value, produit);
        return NoContent();
    }
}