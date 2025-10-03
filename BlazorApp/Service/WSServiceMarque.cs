using BlazorApp.DTO;
using BlazorApp.Service;
using System.Net.Http.Json;

public class WSServiceMarque : IService<MarqueDto>
{
    private readonly HttpClient httpClient;

    public WSServiceMarque(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<MarqueDto>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<MarqueDto>?>("marques/GetAll");
    }

    public async Task<MarqueDto?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<MarqueDto?>($"marques/GetById/{id}");
    }

    public async Task AddAsync(MarqueDto marque)
    {
        await httpClient.PostAsJsonAsync("marques/Create", marque);
    }

    public async Task UpdateAsync(MarqueDto marque)
    {
        await httpClient.PutAsJsonAsync($"marques/Update/{marque.IdMarque}", marque);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"marques/Delete/{id}");
    }
}
