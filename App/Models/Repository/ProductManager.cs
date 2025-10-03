using App.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository;

public class ProductManager : IDataRepository<Produit>
{
    private readonly AppDbContext context;

    public ProductManager(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<ActionResult<IEnumerable<Produit>>> GetAllAsync()
    {
        var produits = await context.Produits
            .Include(p => p.TypeProduitNavigation)
            .Include(p => p.MarqueNavigation)
            .ToListAsync();

        return new ActionResult<IEnumerable<Produit>>(produits);
    }

    public async Task<ActionResult<Produit?>> GetByIdAsync(int id)
    {
        var produit = await context.Produits
            .Include(p => p.TypeProduitNavigation)
            .Include(p => p.MarqueNavigation)
            .FirstOrDefaultAsync(p => p.IdProduit == id);

        return new ActionResult<Produit?>(produit);
    }

    public Task<ActionResult<Produit?>> GetByStringAsync(string str)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Produit entity)
    {
        await context.Produits.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produit entityToUpdate, Produit entity)
    {
        context.Produits.Attach(entityToUpdate);
        context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Produit entity)
    {
        context.Produits.Remove(entity);
        await context.SaveChangesAsync();
    }
}
