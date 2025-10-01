using BlazorApp.Models;
using System.Net.Http.Json;
namespace BlazorApp.Service;

public class WSService : IService<Produit>
{
    private readonly HttpClient httpClient = new() 
    { 
        BaseAddress = new Uri("https://localhost:7008/api/") 
    };

    public async Task AddAsync(Produit produit)
    {
        await httpClient.PostAsJsonAsync<Produit>("produits/Create", produit);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"produits/Delete/{id}");
    }

    public async Task<List<Produit>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Produit>?>("produits/GetAll");
    }

    public async Task<Produit?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<Produit?>($"produits/GetById/{id}");
    }
    public async Task UpdateAsync(Produit updatedEntity)
    {
        await httpClient.PutAsJsonAsync<Produit>($"produits/{updatedEntity.IdProduit}", updatedEntity);
    }
}