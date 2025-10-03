using BlazorApp.DTO;
using BlazorApp.Service;
using System.Net.Http.Json;

public class WSServiceProduit : IService<ProduitDto>
{
    private readonly HttpClient httpClient;

    public WSServiceProduit(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<ProduitDto>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<ProduitDto>?>("produits/GetAll");
    }

    public async Task<ProduitDto?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<ProduitDto?>($"produits/GetById/{id}");
    }

    public async Task AddAsync(ProduitDto produit)
    {
        await httpClient.PostAsJsonAsync("produits/Create", produit);
    }

    public async Task UpdateAsync(ProduitDto produit)
    {
        await httpClient.PutAsJsonAsync($"produits/Update/{produit.Id}", produit);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"produits/Delete/{id}");
    }
}
