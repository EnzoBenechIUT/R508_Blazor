using App.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository;

public class TypeProduitManager(AppDbContext context) : IDataRepository<TypeProduit>
{
    public async Task<ActionResult<IEnumerable<TypeProduit>>> GetAllAsync()
        => await context.TypeProduits.ToListAsync();

    public async Task<ActionResult<TypeProduit?>> GetByIdAsync(int id)
        => await context.TypeProduits.FindAsync(id);

    public async Task AddAsync(TypeProduit entity)
    {
        await context.TypeProduits.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TypeProduit entityToUpdate, TypeProduit entity)
    {
        context.TypeProduits.Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TypeProduit entity)
    {
        context.TypeProduits.Remove(entity);
        await context.SaveChangesAsync();
    }
}
