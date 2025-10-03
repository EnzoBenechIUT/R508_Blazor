using BlazorApp.DTO;
using BlazorApp.Service;
using System.Net.Http.Json;

public class WSServiceTypeProduit : IService<TypeProduitDto>
{
    private readonly HttpClient httpClient;

    public WSServiceTypeProduit(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<TypeProduitDto>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<TypeProduitDto>?>("typeproduits/GetAll");
    }

    public async Task<TypeProduitDto?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<TypeProduitDto?>($"typeproduits/GetById/{id}");
    }

    public async Task AddAsync(TypeProduitDto typeProduit)
    {
        await httpClient.PostAsJsonAsync("typeproduits/Create", typeProduit);
    }

    public async Task UpdateAsync(TypeProduitDto typeProduit)
    {
        await httpClient.PutAsJsonAsync($"typeproduits/Update/{typeProduit.IdTypeProduit}", typeProduit);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"typeproduits/Delete/{id}");
    }
}
